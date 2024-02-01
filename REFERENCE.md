# HTML Language Specifications
Last updated: 2023-17-22.

## Introduction
The HTML (HyperText Machine Language) language is the standard used to create and structure applications inspired by HTML (HyperText Markup Language), referred to as "HTML Classic" or "HTMLC" in this document. This document describes the specifications of the HTML language, including its syntax, elements, attributes, and key features.

## Syntax
HTML is based on `HTML Classic` tags that are used to mark and structure content. 

It is important to note that `HtmlGo` builds upon this foundation and assumes familiarity with `HTML Classic` concepts. Developers can refer to the official HTML specification available at https://html.spec.whatwg.org" to gain a deeper understanding of the language and its underlying principles.

In `HtmlGo`, each HTML tag used within the application carries a distinct meaning and functionality. Unlike languages focused solely on visual presentation, `HtmlGo` goes beyond visual components and embraces a more functional approach. Each tag serves a specific purpose, enabling developers to create structured and semantically meaningful applications.

By leveraging their existing knowledge of HTML and exploring the established standards, developers can more effectively comprehend the syntax and make informed design decisions when working with `HtmlGo`. This familiarity with `HTML Classic` provides a solid foundation for understanding `HtmlGo`'s structure and applying its features to create powerful and interactive applications.

## Structure
The recommended structure for applications developed in HTML is as follows:

```html
<html>

  <head>
    <title>Title of the application</title>
    <meta charset="UTF-8" />
    <!-- Project settings -->
    <!-- CSS & JS - Used for visual enhacements (WYIWYG) -->
  </head>

  <body>
    <h1>Visual title</h1> <!-- Used for visual enhacements -->

    <h2>Main</h2> <!-- Used for visual enhacements -->
    
    <!-- IMPORTANT: Required. Main section calls (ul). -->
    
    <ul>  
      <!-- Calls -->
    </ul>
  </body>

</html>
```

The minimum structure for applications developed in HTML is as follows:

```html
<html>

  <head>
    <title>Title of the application</title>
    <meta charset="UTF-8" />
  </head>

  <body>
    <ul></ul>
  </body>

</html>
```

## Code Example
Here is an example of HTML code that demonstrates the use of some elements and attributes:

```html
<html>

<head>
  <title>HtmlGo! - Hello world example</title>
  <meta charset="UTF-8" />
  <link rel="stylesheet" href="./css/style.css" />
</head>

<body>
  <h1>Hello world example</h1>

  <h2>Main</h2>
  <ul>
    <li class="comment"><i>This is a comment</i></li>

    <li><u>Log</u> <i class="string">Hello world</i></li>

    <li><u>Log</u> <i class="string">1 + 1 is </i> <i class="solve">1 + 1</i></li>

    <li><u>Log</u> <i class="string">The current timestamp is </i> <i class="call">Date.Timestamp()</i></li>
    
    <li>
      <b>If</b>
      <i class="solve">2 > 1</i>
      <ul data-condition="true">
        <li><u>Log</u> <i class="string">You are in an alternative universe!</i></li>
      </ul>
      <ul data-condition="false">
        <li class="comment"><i>False:</i></li>
        <li><u>Log</u> <i class="string">You're in the same universe as the developer.</i></li>
      </ul>
    </li>
  </ul>
</body>

</html>
```

## Metatags
HtmlGo is compatible with any HTML meta tag, meaning that none of them will affect its functionality. In line with the [HTML semantics](https://html.spec.whatwg.org/multipage/semantics.html#meta-application-name), which allows the inclusion of new meta tags, we have decided to introduce some specific meta tags for HtmlGo.

### New Metatags
* Application version (optional): `htmlgo:application-version`.
  * Possible values: `major.minor.patch`.
  * Default value: `1.0.0`.
  * Given a version number MAJOR.MINOR.PATCH, increment the:
      -  MAJOR version when you make incompatible API changes.
      -  MINOR version when you add functionality in a backward compatible manner.
      -  PATCH version when you make backward compatible bug fixes.
  * Additional labels for pre-release and build metadata are **not supported**.
  * Example: `<meta name='htmlgo:application-version' content='1.0.0' />`.

* Application type (optional): `htmlgo:application-type`.
  * Possible values: `Console`, `WebServer`, `Library`.
  * Default value: `Unknown`.
  * Example: `<meta name='htmlgo:application-type' content='Console' />`.

* Application id (optional): `htmlgo:application-id`.
  * Value type: [GUID](https://en.wikipedia.org/wiki/Universally_unique_identifier).
  * Default value: New GUID per execution.
  * Example: `<meta name='htmlgo:application-id' content='7407be91-b093-4264-8c9e-1688230cb35e' />`.

## Language-related Calls
The language-related calls should be enclosed within a `<b>` tag.
* Import
  * Imports a library.
  Usage:
  ```html
    <li><b>Import</b> <a href="LIBRARY LOCAL PATH"></a>ALIAS</li>
  ```
  Example:
  ```html
    <li><b>Import</b> <a href="./Libraries/example_library.html">Example</a></li>
  ```
* If
  * If statement.
  Usage:
  ```html
    <li>
      <b>If</b>
      <i>CONDITION</i>
      <ul data-condition="CONDITION (true or false)">
        INSTRUCTIONS
      </ul>
    </li>
  ```
  Example: 
  ```html
    <li>
      <b>If</b>
      <i class="solve">1 > 2</i>
      <ul data-condition="true">
        <li><u>Log</u> <i class='string'>1 es mayor que 2</i></li>
      </ul>
      <ul data-condition="false">
        <li><u>Log</u> <i class='string'>1 es menor que 2</i></li>
      </ul>
    </li>
  ```
* EndIf
  * Optional.
* Switch
  * Switch statement.
* EndSwitch
  * Optional.
* While
  * While statement.
* Do
  * Do-While statement.
* For
  * Do-While statement.

## Calls
The calls should be enclosed within a `<u>` tag.

### Global calls
* Call
* Const
* Delete
* Goto
* GotoLine
* Label
* Log
* Return
* Set
* SetTitle
* Swap
* Using

### Console calls
* Console.Clear
* Console.HideCursor
* Console.PeekKey
* Console.ReadKey
* Console.ReadLine
* Console.ShowCursor
* Console.SetCursorPosition

### Date calls
* Date.Timestamp
* Date.TimestampInSeconds

### Environment calls
* Environment.CurrentDirectory
* Environment.GetArgs
* Environment.GetEntryFile
* Environment.GetEntryDirectory
* Environment.GetEnvironmentVariable
* Environment.SetEnvironmentVariable

### List calls
* List.Add
* List.New
* List.Get
* List.GetSize
* List.Remove
* List.RemoveAt
* List.Sort
* List.Swap

### Math calls
* Math.Clamp

### OOP calls
* New

### Plugins calls
* Plugins.Load

### String calls
* String.ToLowerCase
* String.ToUpperCase
* String.ToTitleCase
* String.Trim

### Server calls
* Server.Delete
* Server.Get
* Server.GetCurrentDirectory
* Server.Post
* Server.Put
* Server.SetDefaultContentType
* Server.StaticFiles
* Server.StaticDirectory

### Network calls
* Network.HttpGet

### Threading calls
* Threading.Sleep
* Threading.Increment
* Threading.Decrement

## CSS Keywords
* call
  * Used to set dynamic arguments. For example `<i class='call'>Date.Timestamp()</i>`.
* comment
  * Used in `li` to comment the code. For example `<li class="comment"><i>This is a comment</i></li>`.
  * Also can be used in `ul` to interpret the entire list as a comment by the compiler.
* ignore
  * Used in `li` to ignore the line by the compiler. Useful for `<li><hr /></li>` for example.
  * Also used in `ul` to ignore the entire list by the compiler.
* number
  * Used to set the type of the argument. For example `<i class='number'>5</i>`.
* solve
  * Used to set static math-operation arguments. For example `<i class='solve'>1 + 1</i>`.
  * It accepts nested arguments. For example `<i class='solve'><i class='call'>pet.Age</i> + 1</i>`. 
  * The result of each nested argument is converted to a number.
* string
  * Used to set the type of the argument. For example `<i class='string'>I am an HTML developer.</i>`.
* preprocess
  * When using the 'preprocess' parameter, any word starting with $ will be replaced by the corresponding environment variable, if it exists. For example `<i class='string preprocess'>The value of the PATH variable is $PATH.</i>`.

## Specialized Tags
* ul -
  * Used to list instructions.
  * If you include the `data-label` attribute, you can define a group of instructions as a custom call, allowing you to call it from the main program using its associated label.
* ul > li -
  * Used to specify an instruction in the list.
* ul > li > u -
  * Used to make a call within the instruction.
* ul > li > u + * -
  * Elements like `i`, `a`, `span`, `strong` can be used as parameters for the calls.
* ul > li > b -
  * Used for language-related calls (conditions, loops, etc.).
* ul > li > b + * -
  * Parameters for the call within the instruction.
* ul > li > b + * + ul -
  * List of nested instructions.
* ul > li > b + * + ul > li -
  * Instruction within the nested instruction list.
* ul > li > b + * + ul > li > ... -
  * We can repeat the previously specified structure as many times as necessary.

## Get Involved
We appreciate your interest in `HtmlGo` and welcome pull requests to help us improve and enhance the language. You can contribute to the project on our GitHub repository at https://github.com/lcnvdl/html-go.

## Conclusions
In conclusion, `HtmlGo` offers an innovative and visual way to program applications using familiar HTML tags and structure. With its unique syntax and instruction-based approach, `HtmlGo` provides a refreshing and creative programming experience.

When developing in `HtmlGo`, it is important to follow the recommended structure for applications, as described in the previous section. 

With `HtmlGo`, you have the flexibility to use specialized tags for different types of instructions and calls. Explore the possibilities and experiment with nested instructions to create more complex applications.

We invite you to download `HtmlGo`, give it a try, and become part of our growing developer community. You can find more information, tutorials, and code examples on our official website.

We appreciate your interest in `HtmlGo` and welcome pull requests to help us improve and enhance the language. You can contribute to the project on our GitHub repository at https://github.com/lcnvdl/html-go.

We will continue to add updates and improvements to the language and the documentation, so stay tuned for new features and enhancements.

Thank you for choosing `HtmlGo`!

_The HtmlGo Team_