var app = require('express')();
var server = require('http').Server(app);
var io = require('socket.io')(server);
var shortId = require('shortid');

var RandomNum = Math.floor(Math.random() * 100);




var numberPlayerInServer = 0;

var numberPlayerReadyInServer = 0;

var numberPlayerLeft = 0;

var gameIsPlaying = false;

var numberAns = 0;

var allPlayerAns = false;

var numberPlayerWin = 0;
var numberPlayerLose = 0;

var IsRunning = false;




server.listen(8081, function () {
    console.log("server started");
    
});

io.on('connection', function (socket) {

    socket.emit('Connection To Server');

    socket.on('login', function (data) {

        if(!gameIsPlaying)
        {
            numberPlayerInServer++;

            var thisPlayerId = shortId.generate();

            var playerID = {id:thisPlayerId,name:data.playerName,numberplayer:numberPlayerInServer,numberplayerReady:numberPlayerReadyInServer}// Client // JSONobject.AddField("name",playerName);


            console.log("client Connect id = ", playerID);

            console.log(numberPlayerInServer);

            socket.emit('login success', playerID);

            socket.broadcast.emit('other player login', playerID);
        }
        
    });

    socket.on('logout', function (data) {

        numberPlayerInServer--;
        if(data.checkReady == true)
        {
            numberPlayerReadyInServer--;
        }

        console.log(numberPlayerInServer);
        //console.log(numberPlayerReadyInServer);

        
        socket.emit('logout success', data);
        socket.broadcast.emit('other player logout', data);
        //console.log("Logout");
    });

    socket.on("CheckReady", function (data){

        if(!gameIsPlaying)
        {
            if(data.checkReady == true)
            {
                numberPlayerReadyInServer++;

            }
            else if(data.checkReady == false)
            {
                numberPlayerReadyInServer--;
            }

            if(numberPlayerReadyInServer >= numberPlayerInServer)
            {
                var readyStatus = {status:"All Player Ready",playernum:numberPlayerReadyInServer}
                gameIsPlaying = true;
                RandomNum = Math.floor(Math.random() * 6);
                if(RandomNum == 0)
                {
                    RandomNum++;
                }
                console.log(RandomNum);
            }
            else
            {
                var readyStatus = {status:"" + "Player Not Ready " + (numberPlayerInServer - numberPlayerReadyInServer),playernum:numberPlayerReadyInServer}
            }
            //console.log(numberPlayerReadyInServer);
            socket.emit('checkReady', readyStatus);
            socket.broadcast.emit('checkReady', readyStatus);
        }
    });

    socket.on("CheckAns", function (data){

        if(!allPlayerAns)
        {
            numberAns++;

            //console.log("AAA");

            if(data.Ans == true && RandomNum >= 4)
            {
                var conclude = true;
                numberPlayerWin++;
            }
            else if(data.Ans == true && RandomNum <= 3)
            {
                var conclude = false;
                numberPlayerLose++;
            }
            else if(data.Ans == false && RandomNum >= 4)
            {
                var conclude = false;
                numberPlayerLose++;
            }
            else if(data.Ans == false && RandomNum <= 3)
            {
                var conclude = true;
                numberPlayerWin++;
            }
            
            //console.log("BBB");
            //console.log(data.Ans);
            //console.log(numberPlayerWin);
            //console.log(numberPlayerLose);
            
            if(numberAns >= numberPlayerReadyInServer)
            {
                if(numberPlayerWin == 0)
                {
                    numberAns = 0;
                    allPlayerAns = true;
                    var senderValue = {Con:conclude,Message:"No One Win",whatLeft:numberPlayerReadyInServer,id:data.id,Roll:RandomNum}
                }
                else if(numberPlayerWin == 1)
                {
                    numberAns = 0;
                    allPlayerAns = true;
                    var senderValue = {Con:conclude,Message:"Have A Winner",whatLeft:numberPlayerReadyInServer,id:data.id,Roll:RandomNum}
                }
                else
                {
                    numberPlayerReadyInServer = numberPlayerReadyInServer - numberPlayerLose;
                    numberAns = 0;
                    allPlayerAns = true;
                    var senderValue = {Con:conclude,Message:"All Player Answer",whatLeft:numberPlayerReadyInServer,id:data.id,Roll:RandomNum}
                }

                numberPlayerWin = 0;
                numberPlayerLose = 0;

                
            }
            else
            {
                var senderValue = {Con:conclude,Message:"" + (numberPlayerReadyInServer - numberAns) +" Player Not Answer",whatLeft:numberPlayerReadyInServer,id:data.id,Roll:RandomNum}
            }

            //console.log("CCC");

            socket.emit('CheckAnss', senderValue);
            socket.broadcast.emit('CheckAnss', senderValue);
        }
    });

    socket.on("Next Round", function (data){

        //console.log("AAA");
        

        if(!IsRunning)
        {
            RandomNum = Math.floor(Math.random() * 6);
            if(RandomNum == 0)
            {
                RandomNum++;
            }
            IsRunning = true;
            console.log(RandomNum);
        }

        allPlayerAns = false;
        
    });

    socket.on("Remath", function (data){

        if(gameIsPlaying)
        {
            numberPlayerReadyInServer = 0;
            numberAns = 0;
            allPlayerAns = false;
            gameIsPlaying = false;
            IsRunning = false;
            socket.emit('GameEND');
            socket.broadcast.emit('GameEND');
        }
    });

    socket.on("ShowWinner", function (data){

        if(gameIsPlaying)
        {
            numberPlayerReadyInServer = 0;
            numberAns = 0;
            allPlayerAns = false;
            gameIsPlaying = false;
            IsRunning = false;


            socket.emit('ShowWinner',{id:data.id,name:data.name});
            socket.broadcast.emit('ShowWinner',{id:data.id,name:data.name});
        }
    });
});

