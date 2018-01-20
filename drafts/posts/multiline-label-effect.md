Title: Xamarin Platform Effects
Drafted: 01/19/2018
Published: 01/19/2018
Tags:
 - Xamarin Forms
---


# Xamarin Effects
With the newer versions of Xamarin, [Platform Effects](https://developer.xamarin.com/guides/xamarin-forms/application-fundamentals/effects/) have become the recommended approach to handle simple rendering concerns.  Platform Effects are supposed to get you the capabilities of [Custom Renderers](https://developer.xamarin.com/guides/xamarin-forms/application-fundamentals/custom-renderer/) at the individual control level.  This allows you the power to customize individual controls on your page, as opposed to having to customize the rendering of the entire page.

So when I had the requirement of providing a word-wrapped truncated label I figured I could write a Platform Effect that would, save me a custom renderer and use Xamarin's new recommended approach for the task.  I found a thread on the [Xamarin Forums](https://forums.xamarin.com/discussion/26655/multi-line-label-with-tail-truncation) that explained exactly how to solve my problem using a custom renderer.  All I had to do was change my thinking and implement the Platform Effect.

# So my control is null?!

I got my effect all wired up and ready to go, I set my break point in my platform code, run it and what do I find?  A blank control!!!  The aggravation of trying something new that isn't working starts setting in.  I should have just stuck with the example code, atleast we know that worked!  How can the Platform Effect documentation be wrong? I had followed [Adam Pedley's Example](https://xamarinhelp.com/xamarin-forms-triggers-behaviors-effects/) what could be the problem?!  I followed it exactly!  Well, not quite.

I attempted to use the following code on iOS to render my PlatformEffect, which resulted in a null control.

```
MultilineEffect : PlatformEffect<UIView, UILabel>
{
    protected override void OnAttached()
    {
        var effect = (MultilineEffect) Element.Effects.FirstOrDefault(e => e is MultilineEffect);
            if (effect != null)
            {
                Control.Lines = effect.Lines;
            }
    }

    protected override void OnDetached()
    {
    }
}
```

I didn't bother to dig into the Xamarin library to figure out why (mainly because I was crunched for time trying to crank out a feature on Friday), but for some reason when I type constrain the effect, the control and container both returned null.  After a bit more thinking, I figured I could cast out the control without type constraining the effect and acheive the same result.

# Multiline Label Effect

Routing Effect
```
    public class MultilineEffect : RoutingEffect
    {
        public int Lines { get; set; }

        public MultilineEffect() : base("Company.MultilineTruncateLabelEffect")
        {
        }
    }
```

iOS Effect
```
    public class MultilineTruncateLabelEffect : PlatformEffect
    {
        readonly Func<Element,MultilineEffect> GetEffect = (element) => (MultilineEffect) element.Effects.FirstOrDefault(e => e is MultilineEffect);

        protected override void OnAttached()
        {
            try
            {
                var effect = GetEffect(Element);
                if (effect != null)
                {
                    ((UILabel)Control).Lines = effect.Lines;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
        }
    }
```

Android Effect
```
    public class MultilineTruncateLabelEffect : PlatformEffect
    {
        readonly Func<Element,MultilineEffect> GetEffect = (element) => (MultilineEffect) element.Effects.FirstOrDefault(e => e is MultilineEffect);

        protected override void OnAttached()
        {
            // TextView
            try
            {
                var effect = GetEffect(Element);
                if (effect != null)
                {
                    ((TextView)Control).SetSingleLine(false);
                    ((TextView)Control).SetLines(effect.Lines);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
        }
    }
```

# Consume it in XAML
At this point all that was left was to consume it in XAML.  The custom renderer on the forums created a custom label type that took in an integer to dictate how many lines we were requesting for the label.  Based on the way the Platform Effect takes [CLR properites](https://developer.xamarin.com/guides/xamarin-forms/application-fundamentals/effects/passing-parameters/clr-properties/)  as parameters, I don't need a custom label type to make this work.  I can now attach this on any label, pass in the number of lines and I have a multiline word-wrapped label, that still respects the `LineBreakMode` property.

``` xaml
<Label Text="N/A"
        LineBreakMode="TailTruncation">
    <Label.Effects>
        <local:MultilineEffect Lines="2" />
    </Label.Effects>
</Label>
```