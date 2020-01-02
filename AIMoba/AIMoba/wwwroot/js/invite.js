"use strict";

let connection = new signalR.HubConnectionBuilder()
    .withUrl("/lobbyHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

document.getElementById("inviteButton").disabled = true;
document.getElementById("readyButton").disabled = true;
document.getElementById("startButton").disabled = true;



connection.start().then(function () {
    console.log("feliratkozas");
    connection.invoke("Subscribe", document.getElementById("name").innerText).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("inviteButton").disabled = false;
    document.getElementById("readyButton").disabled = false;
}).catch(function (err) {

    return console.error(err.toString());
});

connection.on("PlayerJoined", function (message) {
    console.log(message);
});

document.getElementById("inviteButton").addEventListener("click", function (event) {
    let name = document.getElementById("inviteName").value;
    let roomName = document.getElementById('roomName').innerText;
    console.log(name, roomName);
    connection.invoke("InvitePlayer", name, roomName).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
});

document.getElementById("readyButton").addEventListener("click", function (event) {
    let myname = document.getElementById("name").innerText;
    let roomName = document.getElementById('roomName').innerText;

    connection.invoke("JoinLobby", roomName, myname).then(() => document.getElementById("startButton").disabled = false)
        .catch(err => console.error(err.toString()));

    event.preventDefault();
});

document.getElementById("startButton").addEventListener("click", function (event) {
    let roomName = document.getElementById('roomName').innerText;

    connection.invoke("StartGame", roomName)
        .catch(err => console.error(err.toString()));

    event.preventDefault();
});

connection.on("StartGame", (link) => {
    let myname = document.getElementById("name").innerText;
    console.log(link);
    window.location = link+"/"+myname;
})

