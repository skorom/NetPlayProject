"use strict";

connection.on("ReceiveMessage", function (user, message) {
    let msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    let encodedMsg= user + " says " + msg;
    document.getElementById("messageList").value += encodedMsg + "\n";
    document.getElementById('messageList').scrollTop = document.getElementById('messageList').scrollHeight;
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    let message = document.getElementById("messageInput").value;
    let player = document.getElementById("playerName").value;
    let user = player.substring(6, name.indexOf('!'));
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});