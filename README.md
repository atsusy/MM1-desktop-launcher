#  MM1 desktop launcher
Simple Desktop App Launcher for Windows 10

![Demo](demo.gif)

# Features
- Dark/Light theme
- Customaizable Hot key
- Launch Store/Desktop apps
- Custom URIs
- Extra Folders

# Custom URI parameter
You can pass parameter to URI with `{[number]}`.
For example, regsiter the URI `https://www.google.co.jp/search?q={1}` with name `g` and enter `g test`, The URI will be extracted as `https://www.google.co.jp/search?q=test`

# Extensions for Extra Folders
You can pass file extensions extra folder searching for.
## Extensions format
```
extension = "*."<extension-name>
extensions = <extension> | <extension>"|"<extensions>
```

# OS
- Windows 10

# License
MIT Licsense

# Privacy Policy
[Privacy Policy](privacy_policy.html)
