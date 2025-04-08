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
- 📏 All `{{ variable }}` tokens must be on their **own line**  
  ✅ Allowed:  
  ```html
  <p>
      {{ Person.Name }}
  </p>
  ```  
  ❌ Not allowed:  
  ```html
  <p>Name: {{ Person.Name }}</p>
  ```

---

## 📐 Template Syntax

### 1. **Direct Binding**
```html
<p>
    {{ User.Name }}
</p>
```

### 2. **Method Calls**
```html
<p>
    {{ Report.GetFormattedDate }}
</p>
```

### 3. **Conditional Rendering (`pdf-if`)**
```html
<pdf-if>
    % User.Age < 18 %
    {
        <p>
            {{ User.Name }}
        </p>
    }

    % User.Age >= 21 %
    {
        <p>
            {{ User.Name }}
        </p>
    }

    % else %
    {
        <p>
            {{ User.Name }}
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

### `PDFCallAttribute`

```csharp
[PDFCall]
public string GetFormattedDate() {
    return DateCreated.ToString("MMMM dd, yyyy");
}
```

You can also apply this to computed properties:
```csharp
[PDFCall]
public string StatusLabel => IsActive ? "Active" : "Inactive";
```

---

## 🛠 Usage

```csharp
var engine = new TemplateEngine();
var html = File.ReadAllText("template.html");

var result = engine.Render(html, new {
    Person = person,
    Report = report
});

engine.GeneratePdf(result, "output.pdf");
```

---

## 📎 Notes

- Templates must follow strict formatting rules (see above)
- Internally, uses `wkhtmltopdf` for rendering
- Currently does **not support nested** `pdf-if`, `pdf-for`, or `pdf-foreach`

---

## 📃 License

MIT License
