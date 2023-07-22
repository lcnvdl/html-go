# HtmlGo changelog
## v0.4.0-alpha (next)
* Memory heap. Reference values. Introducing support for a memory heap to handle reference values, enabling the use of variables by reference instead of just value-based variables
* Enhanced support for nested elements: improved support for nested elements in the `solve` argument type, allowing for more complex and nested expressions.
* Improved CSS for tables: they do not occupy 100% width by default.

## v0.3.0-alpha
* Runtime parameters:
  * Version info. Usage: `htmlgo --version` or `htmlgo-server --version`.
  * Https redirection (disabled by default). Usage: `htmlgo-server --http Example1.html`.
  * String provider (String.Trim, String.ToLowerCase, String.ToUpperCase, String.ToTitleCase).
  * Terminal provider (Console.ReadLine).
  * Css "calls" recommendation removed.
  * Call instruction added. Now you can call functions from different instruction groups.

**Full Changelog**: https://github.com/lcnvdl/html-go/compare/v0.2.0-alpha...v0.3.0-alpha

## v0.2.0-alpha
* Improvement: Enhance Instruction Processing. [View issue](https://github.com/lcnvdl/html-go/issues/5).
* Selection statements (switch).
* Iteration statements (while, do-while, for).

**Full Changelog**: https://github.com/lcnvdl/html-go/compare/v0.1.0-alpha...v0.2.0-alpha

## v0.1.0-alpha
* First version.

**Full Changelog**: https://github.com/lcnvdl/html-go/commits/v0.1.0-alpha
