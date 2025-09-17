// See https://aka.ms/new-console-template for more information
using Eto.Drawing;
using Eto.Forms;
using System.Reflection;
Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

Console.WriteLine("Hello, World!");



var app = new Application(Eto.Platforms.WinForms);
var form = new Form
{
    Title = "Box KeyUp Demo",
    ClientSize = new Size(640, 400)
};

Drawable MakeBox(int number, Color color)
{
    var d = new Drawable
    {
        BackgroundColor = Colors.Transparent,
        MinimumSize = new Size(140, 120),
        Tag = number,
        TabIndex = number
    };
    System.Console.WriteLine(d.ControlObject.GetType());


    Type t = d.ControlObject.GetType();

    Console.WriteLine($"Type: {t.FullName}");
    Console.WriteLine($"Base Type: {t.BaseType?.FullName ?? "None"}");

    Console.WriteLine("\nInterfaces:");
    foreach (var iface in t.GetInterfaces())
        Console.WriteLine($"  - {iface.FullName}");

    Console.WriteLine("\nProperties:");
    foreach (var prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        Console.WriteLine($"  - {prop.PropertyType.Name} {prop.Name}");

    d.Paint += (sender, e) =>
    {
        var rect = new RectangleF(d.Bounds.Location, d.Bounds.Size);
        rect.Inflate(-8, -8);

        e.Graphics.FillRectangle(color, rect);
        e.Graphics.DrawRectangle(Colors.Black, rect);
        e.Graphics.DrawText(SystemFonts.Bold(14), Colors.White, rect.Left + 10, rect.Top + 10, $"Box {number}");
    };

    if(d.ControlObject is System.Windows.Forms.Control x){
        System.Console.WriteLine("Cast as Control");
        x.TabIndex = number;
    }

    // Each box listens for its own KeyUp
    d.KeyUp += (sender, e) =>
    {
        var keyName = e.Key.ToString();
        var keyCode = (int)e.Key;
        form.Title = $"Box {number} received KeyUp: {keyName} ({keyCode})";
    };

    // Make it focusable so it can receive key events
    d.CanFocus = true;

    // Click to focus the box
    d.MouseDown += (sender, e) => d.Focus();

    return d;
}

var boxes = new[]
{
    MakeBox(1, Colors.CornflowerBlue),
    MakeBox(2, Colors.OrangeRed),
    MakeBox(3, Colors.SeaGreen),
    MakeBox(4, Colors.Goldenrod)
};

var stack1 = new StackLayout
{
    Orientation = Orientation.Horizontal,
    Spacing = 10,
    Items = { boxes[0], boxes[1] }
};

var stack2 = new StackLayout
{
    Orientation = Orientation.Horizontal,
    Spacing = 10,
    Items = { boxes[2], boxes[3] }
};

form.Content = new StackLayout
{
    Orientation = Orientation.Vertical,
    Spacing = 12,
    Padding = 12,
    Items = { stack1, stack2 }
};

boxes[0].Focus();
app.Run(form);
