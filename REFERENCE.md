# HTML Language Specifications
Last updated: 2023-06-07.

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
    <!-- IMPORTANT: Recommended. Add the .calls class to the ul element to emphasize it as the entry point of the application. -->"
    
    <ul class="calls">  
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
    <ul class="calls"></ul>
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
  <ul class="calls">
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

## CSS Keywords
* call
  * Used to set dynamic arguments. For example `<i class='call'>Date.Timestamp()</i>`.
* calls
  * Used in `ul` to specify an instruction list.
* comment
  * Used in `li` to comment the code. For example `<li class="comment"><i>This is a comment</i></li>`.
* ignore
  * Used in `li` to ignore the line by the compiler. Useful for `<li><hr /></li>` for example.
* number
  * Used to set the type of the argument. For example `<i class='number'>5</i>`.
* solve
  * Used to set static math-operation arguments. For example `<i class='solve'>1 + 1</i>`.
* string
  * Used to set the type of the argument. For example `<i class='string'>I am an HTML developer.</i>`.

## Specialized Tags
* ul -
  * Used to list instructions.
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

When developing in `HtmlGo`, it is important to follow the recommended structure for applications, as described in the previous section. Additionally, it is recommended to add the .calls class to the ul element to highlight it as the entry point of the application.

With `HtmlGo`, you have the flexibility to use specialized tags for different types of instructions and calls. Explore the possibilities and experiment with nested instructions to create more complex applications.

We invite you to download `HtmlGo`, give it a try, and become part of our growing developer community. You can find more information, tutorials, and code examples on our official website.

We appreciate your interest in `HtmlGo` and welcome pull requests to help us improve and enhance the language. You can contribute to the project on our GitHub repository at https://github.com/lcnvdl/html-go.

We will continue to add updates and improvements to the language and the documentation, so stay tuned for new features and enhancements.

Thank you for choosing `HtmlGo`!

_The HtmlGo Team_