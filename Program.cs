// See https://aka.ms/new-console-template for more information
using Eto.Drawing;
using Eto.Forms;
using System.Reflection;
//Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
//Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

Console.WriteLine("Hello, World!");

AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);

var app = new Application(Eto.Platforms.WinForms);
var form = new Form
{
    Title = "Box KeyUp Demo",
    ClientSize = new Size(640, 400)
};



FontFamily fam = FontFamilies.Sans;
var rnd = new Random();
Color[] palette = { Colors.AliceBlue, Colors.SteelBlue, Colors.Coral, Colors.MediumSeaGreen, Colors.Plum, Colors.Gold, Colors.LightGrey };

TextBox[] GenTextBoxes(int n, string prefix = "V") =>
    Enumerable.Range(1, n).Select(i =>
        new TextBox { Width = 200, BackgroundColor = palette[rnd.Next(palette.Length)], Font = new Font(fam, 10f), Text = $"{prefix}{i}" })
    .ToArray();

TextArea[] GenTextAreas(int n, string prefix = "T") =>
    Enumerable.Range(1, n).Select(i =>
        new TextArea { Width = 200, Height = 60, BackgroundColor = palette[rnd.Next(palette.Length)], Font = new Font(fam, 11f), Text = $"{prefix}{i}" })
    .ToArray();

Button[] GenButtons(int n, string prefix = "T") =>
    Enumerable.Range(1, n).Select(i =>
        new Button { Width = 200, Height = 60, BackgroundColor = palette[rnd.Next(palette.Length)], Font = new Font(fam, 11f), Text = $"{prefix}{i}" })
    .ToArray();

StackLayout GenMixedLayout(int tbCount, int taCount, int perRow = 2)
{
    var tbs = GenTextBoxes(tbCount, "TB");
    var bs = GenButtons(tbCount, "TB");
    var tas = GenTextAreas(taCount, "TA");
    var max = Math.Max(tbs.Length, tas.Length);
    var items = Enumerable.Range(0, max * 3)
        .Select(i =>
        {
            var idx = i / 3;
            return (Control)(i % 3 == 0 ? (idx < tbs.Length ? tbs[idx] : new Label())
                                : i % 3 == 1 ? (idx < tas.Length ? tas[idx] : new Label())
                                : (idx < bs.Length ? bs[idx] : new Label()));
        })
        .ToArray();

    var outer = new StackLayout { Orientation = Orientation.Vertical, Spacing = 6 };
    for (int i = 0; i < items.Length; i += perRow)
    {
        var row = new StackLayout { Orientation = Orientation.Horizontal, Spacing = 6 };
        foreach (var c in items.Skip(i).Take(perRow)) row.Items.Add(c);
        outer.Items.Add(row);
    }
    return outer;
}

// Examples:
var boxesT = GenTextBoxes(4);
var areas = GenTextAreas(2);
var mixed = GenMixedLayout(3, 2, perRow: 3);
var mixed2 = GenMixedLayout(3, 2, perRow: 3);
var mixed3 = GenMixedLayout(3, 2, perRow: 3);
Drawable MakeBox(int number, Color color)
{
    var d = new Drawable
    {
        BackgroundColor = Colors.Transparent,
        MinimumSize = new Size(140, 120),
        Tag = number,
        //Following: demonstrates that TabIndex is not necessary to receive keyboard focus.
        //TabIndex = number,
        
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
    d.Content = GenMixedLayout(4, 4, 3);

    if(d.ControlObject is System.Windows.Forms.Control x){
        System.Console.WriteLine("Cast as Control");
        //x.TabIndex = number;
        //TabStop is important, otherwise things won't stop there.
        x.TabStop = true;
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
    Items = { boxes[0], boxes[1], mixed }
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
