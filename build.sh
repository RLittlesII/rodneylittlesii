#!/usr/bin/env bash
##########################################################################
# This is the Cake bootstrapper script for Linux and OS X.
# This file was downloaded from https://github.com/cake-build/resources
# Feel free to change this file to fit your needs.
##########################################################################

# Define directories.
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
TOOLS_DIR=$CAKE_PATHS_TOOLS
NUGET_EXE=$SCRIPT_DIR/nuget.exe
NUGET_URL=https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
CAKE_VERSION=0.32.1
CAKE_EXE=$TOOLS_DIR/Cake.$CAKE_VERSION/Cake.exe
DOTNET_PATH=$SCRIPT_DIR/.dotnet
DOTNET_VERSION=2.1.500
WYAM_EXE="~/.dotnet/tools"/wyam.exe
WYAM_VERSION=2.1.3

# Define default arguments.
TARGET="Default"
CONFIGURATION="Release"
VERBOSITY="verbose"
DRYRUN=
SCRIPT_ARGUMENTS=()

# Parse arguments.
for i in "$@"; do
    case $1 in
        -t|--target) TARGET="$2"; shift ;;
        -c|--configuration) CONFIGURATION="$2"; shift ;;
        -v|--verbosity) VERBOSITY="$2"; shift ;;
        -d|--dryrun) DRYRUN="-dryrun" ;;
        --) shift; SCRIPT_ARGUMENTS+=("$@"); break ;;
        *) SCRIPT_ARGUMENTS+=("$1") ;;
    esac
    shift
done

# Make sure the tools folder exist.
if [ ! -d "$TOOLS_DIR" ]; then
  mkdir "$TOOLS_DIR"
fi

###########################################################################
# INSTALL .NET CORE CLI
###########################################################################

echo "Installing .NET CLI..."
if [ ! -d "$DOTNET_PATH" ]; then
  mkdir "$DOTNET_PATH"
fi
curl -Lsfo "$DOTNET_PATH/dotnet-install.sh" https://dot.net/v1/dotnet-install.sh
sudo bash "$DOTNET_PATH/dotnet-install.sh" --version $DOTNET_VERSION --install-dir .dotnet --no-path
export PATH="$DOTNET_PATH":$PATH
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export DOTNET_CLI_TELEMETRY_OPTOUT=1
"$DOTNET_PATH/dotnet" --info

###########################################################################
# INSTALL NUGET
###########################################################################

# Download NuGet if it does not exist.
if [ ! -f "$NUGET_EXE" ]; then
    echo "Downloading NuGet..."
    curl -Lsfo "$NUGET_EXE" $NUGET_URL
    if [ $? -ne 0 ]; then
        echo "An error occured while downloading nuget.exe."
        exit 1
    fi
fi

###########################################################################
# INSTALL WYAM
###########################################################################

# Install Wyam if it does not exist.
if [ ! -f "$WYAM_EXE" ]; then
    echo "Installing Wyam..."
    dotnet tool install -g Wyam.Tool --version $WYAM_VERSION
    export PATH="$WYAM_EXE":$PATH
fi

###########################################################################
# INSTALL CAKE
###########################################################################

if [ ! -f "$CAKE_EXE" ]; then
    echo "Installing Cake..."
    dotnet tool install -g Cake.Tool --version $CAKE_VERSION
fi

###########################################################################
# RUN BUILD SCRIPT
###########################################################################

# Start Cake
dotnet-cake build.cake --verbosity=$VERBOSITY --configuration=$CONFIGURATION --target=$TARGET $DRYRUN "${SCRIPT_ARGUMENTS[@]}"