# HtmlGo!

The interpreter that lets you 'program' in HTML! Now you can finally join the fun and proudly say you're an HTML programmer (sort of)!

¡El intérprete que te permite 'programar' en HTML! Ahora puedes unirte a la diversión y decir con orgullo que eres un programador de HTML (más o menos)!

![alt text](./Docs/screenshot_1.png)

## What is HTML (HyperText __Machine__ Language)?
The HTML (HyperText Machine Language) language is our new standard we've made to to create and structure applications inspired by HTML (HyperText Markup Language).

For a comprehensive understanding of the HTML language and its syntax, we recommend referring to the Language Specification. This specification provides detailed documentation on the language's structure, elements, attributes, and key features.

To access the HTML Language Specification, please visit [REFERENCE.md](./REFERENCE.md).

## What is HtmlGo?
`HtmlGo` allows you to compile and execute applications in a whole new way. Say goodbye to boring lines of code and embrace a more visual and developer-friendly approach. With `HtmlGo`, all you have to do is write your code in .html files and let its interpreter, developed in NET 6.0, do the rest.

Yes, you heard it right, programming in HTML! But don't worry, it's not like the HTML you know. `HtmlGo` offers a unique way to write instructions using the tags and structure of HTML that you're already familiar with. It's like giving a fresh and creative twist to your programming experience.

The `HtmlGo` interpreter reads your .html file and executes the instructions you've created.

Want to be part of this new way of programming? Join the `HtmlGo` developer community and discover how this language can bring a fun and refreshing twist to your projects. Say goodbye to monotony and welcome a more visual, creative, and HTML-infused programming experience!

## How to use?
Here's a quick guide on how to use `HtmlGo` with commands:

1. Open your terminal or command prompt.
2. Run the following command:
```bash
htmlgo /path/to/app.html
```
Make sure to replace "/path/to/app.html" with the actual path and name of your app.html file.

`HtmlGo` will start interpreting the HTML file and executing the instructions you've created.

And that's it! With these simple steps, you'll be able to run your Html application and see the results in action.

## Getting Started for Users
Before you can start using `HtmlGo` to compile and execute applications, you'll need to perform a simple setup process. This involves downloading the `HtmlGo` package, extracting it to a directory, and configuring your system's environment variables. Follow the steps below to get started:

1. Download the zip file containing the `HtmlGo` package.
2. Extract the contents of the zip file to a directory of your choice.
3. Open the system's environment variables settings.
4. Add the directory path where you extracted `HtmlGo` to the PATH environment variable.
  * For Windows:
    * Press Win + X and select "System".
    * Click on "Advanced system settings".
    * In the "System Properties" window, click on the "Environment Variables" button.
    * In the "Environment Variables" window, under "System variables", select "Path" and click on "Edit".
    * Add the directory path to the list of paths, separating each path with a semicolon (;).
    * Click "OK" to save the changes.
  * For macOS and Linux:
    * Open a terminal window.
    * Run the command nano ~/.bash_profile to edit your bash profile.
    * Add the following line at the end of the file: export PATH="/path/to/htmlgo:$PATH", replacing "/path/to/htmlgo" with the actual directory path where you extracted `HtmlGo`.
    * Press Ctrl + X, then Y, and finally Enter to save the changes.

Once you have completed these steps, you have successfully installed `HtmlGo` on your system. You can now start using `HtmlGo` to compile and execute applications written in HTML.

## Tutorials (Spanish)
https://www.youtube.com/@ACodearla

## Dev tools
* Commands (list of commands I've used) - `commands.bat`
* Watch (compile and run) - `watch.bat`
* Publish (release build) - `publish.bat`

## License
[Attribution-NonCommercial-NoDerivatives 4.0 International](https://github.com/lcnvdl/html-go/blob/master/LICENSE)
