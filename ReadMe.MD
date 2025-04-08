# 🧾 PDFTemplateLibrary

**PDFTemplateLibrary** is a lightweight C# library that lets you generate PDFs from clean, HTML-like templates using a custom syntax. It supports templated loops, conditions, method calls, and data binding using familiar C# object models — with no external rendering logic.

> ⚙️ Internally, the library uses [wkhtmltopdf](https://wkhtmltopdf.org/) to render HTML into PDF format.

---

## ✨ Features

- 🔁 `pdf-for`, `pdf-foreach` loops for iterative rendering
- 🔀 `pdf-if` conditional branching
- 🧩 Template values support direct property and method calls
- 📄 HTML-first templates with inline C#-style logic
- ✅ Uses strongly typed C# models — no scripting or Razor engine

---

## ⚠️ Limitations

- ❌ No expression evaluation — you **cannot** use math, logic, or ternary operations inside `{{ }}`
- 📏 Onlt a single `{{ variable }}` token can be on one line **own line**    
  ❌ Not allowed:  
  ```html
  <p>
      Full Name {{ Person.Name }} {{ Person.Surname }}
  </p>
  ```  
  ✅ Allowed:  
  ```html
  <p>Name: {{ Person.Name }}</p>
  ```
---

## 📐 Template Syntax

### 1. **Direct Binding**
```html
<p>
    Current Usernaem: {{ User.Name }}
</p>
```

### 2. **Method Calls**
```html
<p>
    {{ Report.GetFormattedDate() }}
</p>
```

### 3. **Conditional Rendering (`pdf-if`)**
```html
<pdf-if>
    % User.Age < 18 %
    {
        <p>
            {{ User.Name }} is a child
        </p>
    }

    % User.Age >= 21 %
    {
        <p>
            {{ User.Name }} is in their early twenties
        </p>
    }

    % else %
    {
        <p>
            Mr/Mrs. {{ User.Surname }} is old
        </p>
    }
</pdf-if>
```

### 4. **For Loop (`pdf-for`)**
```html
<pdf-for start={0} end={Items.Count} as={i}>
    <p>
        {{ Items[i] }}
    </p>
</pdf-for>
```

### 5. **Foreach Loop (`pdf-foreach`)**
```html
<pdf-foreach collection={User.Friends} as={friend}>
    <p>
        {{ friend.Name }}
    </p>
</pdf-foreach>
```

---

## 🏷️ Attributes

To expose methods or computed properties to the template engine, mark them with:

### `PDFCall` Attribute

```csharp
[PDFCall]
public string GetFormattedDate() {
    return DateCreated.ToString("MMMM dd, yyyy");
}
```

To keep properties for internal method use you can exclude them using the:
### `PDFIgnore` Attribute

```csharp
[PDFIgnore] public int ItemModifier { get; set; }
public int CostPrice { get; set; }
[PDFIgnore] public char CurrencySymbol { get; set; }

[PDFCall] public string GetTotalPrice() {
    int finalPrice = CostPrice * ItemModifier;
    string formattedPrice = $"{CurrencySymbol} {finalPrice}";
    return formattedPrice;
}

```

---

## 🛠 Usage

```csharp
public static void Main() {
    ProjectReport report = new()
    {
        Title = "Website Redesign",
        Manager = "Ava Johnson",
        DateCreated = DateTime.Now,
        Status = "In Progress",
        TeamMembers = new List<string> { "Ava Johnson", "Liam Smith", "Emma Davis", "Noah Lee" },
        Tasks = new List<TaskItem>
        {
            new TaskItem { Title = "Create wireframes", AssignedTo = "Emma Davis", IsCompleted = true },
            new TaskItem { Title = "Build landing page", AssignedTo = "Noah Lee", IsCompleted = false },
            new TaskItem { Title = "Write content", AssignedTo = "Liam Smith", IsCompleted = false },
            new TaskItem { Title = "Client review", AssignedTo = "Ava Johnson", IsCompleted = false },
        }
    };

    PDFDocument document = TemplateRenderer.RenderDocument("ProjectReport/ProjectReport.html", "ProjectReport/ProjectReportPDF", report);
    File.WriteAllBytes(document.FileFullName, document.PDF); // Write the PDF file as bytes
}
```

---

## 📎 Notes

- Templates must follow strict formatting rules (see above)
- Internally, uses `wkhtmltopdf` for rendering
- Currently does **not support nested** `pdf-if`, `pdf-for`, or `pdf-foreach`

---

## 📃 License

MIT License
