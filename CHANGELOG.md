# HtmlGo changelog
## Next
* List calls. Now you can manipulate lists.
* Network calls. Now you can send http requests.
* New string instruction: `Split`.
* New variable instruction: `Swap`. It allows you to swap values between variables in one single line.

## v0.4.0-alpha
* Memory heap. Reference values. Introducing support for a memory heap to handle reference values, enabling the use of variables by reference instead of just value-based variables
* New `meta` tags support: `htmlgo:application-id`, `htmlgo:application-version`, `htmlgo:application-type`.
* Library generation and import.
* Enhanced support for nested elements: improved support for nested elements in the `solve` argument type, allowing for more complex and nested expressions.
* Improved CSS for tables: they do not occupy 100% width by default.

**Full Changelog**: https://github.com/lcnvdl/html-go/compare/v0.3.0-alpha...v0.4.0-alpha

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
