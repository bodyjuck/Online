var app = require('express')();
var server = require('http').Server(app);
var io = require('socket.io')(server);
var shortId = require('shortid');

var players = [];
var protectedRepeatID = [];

server.listen(8081, function () {
    console.log("server started");
});

io.on('connection', function (socket) {

    socket.emit('Connection To Server');

    socket.on("CheckStatus", function (data){
        socket.emit('Connection To Server');
    });

    

    socket.on('login', function (data) {

        do
        {
            var thisPlayerId = shortId.generate();
        }
        while(thisPlayerId == protectedRepeatID[thisPlayerId]);

        protectedRepeatID[thisPlayerId] = thisPlayerId;

        var playerID = {id:thisPlayerId,name:data.playerName}// Client // JSONobject.AddField("name",playerName);
        players[thisPlayerId] = playerID;

        console.log("client Connect id = ", playerID);

        socket.emit('login success', playerID);
        socket.broadcast.emit('other player login', playerID);
    });

    socket.on('logout', function (data) {

        protectedRepeatID[data.id] = "";
        players[data.id] = "";

        socket.emit('logout success', data);
        socket.broadcast.emit('other player logout', data);
    });

});

