# Look at our Slack (LaoS)
A slack app which exposes one channel to a websocket so people can read allong on your website. Only read rights, no interaction back.
Features
* Reads from one slack channel
* Pure websockets
* Simple markup and Emoij parsing
* Delete and edit events from slack are honoured 
* De-duplication of messages
* In memory persistance 

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
2. now alias {new deployment} laos 
3. now rm {oldest deployment}
If you want send me tips on how to improve this

### TODO
0. Percistance of messages to Azure storage
1. Handeling of images 
2. Handeling of reactions on a message 
3. Handeling of links
4. Handeling of other attachements
5. propper styling and JS code (frontend optimalisation)
6. Simple Interaction mode
7. Account system for adding new channels :D

### Thanks to
 - [dotNetCore](https://www.microsoft.com/net)
 - [Moment.js](https://momentjs.com/)
 - [iamcal for js emoji](https://github.com/iamcal/js-emoji)
 - [joewalnes for reconnecting-websocket](https://github.com/joewalnes/reconnecting-websocket)
