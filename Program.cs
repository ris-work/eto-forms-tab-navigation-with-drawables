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
        new TextBox { Width = 100, BackgroundColor = palette[rnd.Next(palette.Length)], Font = new Font(fam, 10f), Text = $"{prefix}{i}" })
    .ToArray();

TextArea[] GenTextAreas(int n, string prefix = "T") =>
    Enumerable.Range(1, n).Select(i =>
        new TextArea { Width = 100, Height = 60, BackgroundColor = palette[rnd.Next(palette.Length)], Font = new Font(fam, 11f), Text = $"{prefix}{i}" })
    .ToArray();

Button[] GenButtons(int n, string prefix = "T") =>
    Enumerable.Range(1, n).Select(i =>
        new Button { Width = 100, Height = 60, BackgroundColor = palette[rnd.Next(palette.Length)], Font = new Font(fam, 11f), Text = $"{prefix}{i}" })
    .ToArray();

Panel[] GenPanels(int n, string prefix = "T") =>
    Enumerable.Range(1, n).Select(i =>
        new Panel { Width = 100, Height = 60, BackgroundColor = palette[rnd.Next(palette.Length)]})
    .ToArray();

DateTimePicker[] GenDateControl(int n, string prefix = "T") =>
    Enumerable.Range(1, n).Select(i =>
        new DateTimePicker { Width = 100, Height = 60, BackgroundColor = palette[rnd.Next(palette.Length)], Font = new Font(fam, 11f) })
    .ToArray();

RadioButton[] GenRB(int n, string prefix = "T") =>
    Enumerable.Range(1, n).Select(i =>
        new RadioButton { Width = 100, Height = 60, BackgroundColor = palette[rnd.Next(palette.Length)], Font = new Font(fam, 11f), Text = $"{prefix}{i}" })
    .ToArray();

StackLayout GenMixedLayout(int tbCount, int taCount, int perRow = 2)
{
    var tbs = GenTextBoxes(tbCount, "TB");
    var bs = GenButtons(tbCount, "TB");
    var tas = GenTextAreas(taCount, "TA");

    // additional generators provided by user
    var ps = GenPanels(Math.Max(0, (tbCount + taCount) / 2), "P");          // example count derived from inputs
    var dps = GenDateControl(Math.Max(0, (tbCount + taCount) / 3), "D");    // example count derived from inputs
    var rbs = GenRB(Math.Max(0, (tbCount + taCount) / 3), "R");             // example count derived from inputs

    // Combine all arrays into a single round-robin sequence so tests exercise all types.
    var lists = new Control[][] { tbs, tas, bs, ps, dps, rbs };
    var maxLen = lists.Max(a => a.Length);
    var items = new List<Control>(maxLen * lists.Length);

    for (int i = 0; i < maxLen; i++)
    {
        foreach (var arr in lists)
        {
            if (i < arr.Length) items.Add(arr[i]);
            else items.Add(new Label()); // placeholder when a sequence is exhausted
        }
    }

    var outer = new StackLayout { Orientation = Orientation.Vertical, Spacing = 6 };
    for (int i = 0; i < items.Count; i += perRow)
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
var mixed = GenMixedLayout(2, 2, perRow: 2);
var mixed2 = GenMixedLayout(2, 2, perRow: 2);
var mixed3 = GenMixedLayout(2, 2, perRow: 2);
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
    d.Content = GenMixedLayout(4, 2, 3);

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
