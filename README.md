# Look at our Slack (LaoS)
A slack app which exposes one channel to a websocket so people can read allong on your website. Only read rights, no interaction back.

### Developed 
Developed on dotNetCore 1.1 with [Nancy FX ](http://nancyfx.org/) 
It uses part Nancy, part custom middleware for the websockets.
Storage is done with Azure storage, see the *using the code* section for instructions

### Using the code
Feel free to fork or critique my work, it's just a hobby project :D
When using please add this appsettings.json your self with your own key and account;

{
  "storageKey": "y",
  "storageAccount": "x"
}

### Deployment to [Zeit](https://zeit.co/)
On windows machines please use the release.ps1, on linux.. please enter the commands your self
After release clean up the old code by first
1. now ls (list all deployments)
2. now alias {new deployment} laos // or any other name
3. now rm {oldest deployment}
