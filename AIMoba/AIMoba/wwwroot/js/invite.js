"use strict";

// a szerver kapcsolat
let connection = new signalR.HubConnectionBuilder()
    .withUrl("/lobbyHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

// amíg a szerverrrel való kapcsolat nem jön létre addig a gombok nem akívak
document.getElementById("inviteButton").disabled = true;
document.getElementById("readyButton").disabled = true;
document.getElementById("startButton").disabled = true;


// a szerver kapcsolat kiépítése
document.addEventListener('DOMContentLoaded', () => {
    connection.start()
        .then(() => { // miután a kapcsolat létrejött

            let myName = document.getElementById("name").innerText;
            let roomName = document.getElementById('roomName').innerText;

            // feliratkozás a szerverre, a megfelelő id - név kapcsolat létrehozásáért
            connection.invoke("Subscribe", myName)
                .catch((err) => console.error(err.toString()));

            // csatlakozás a jelenlegi szoba lobby - ához
            connection.invoke("JoinLobby", roomName, myName)
                .catch(err => console.error(err.toString()));

            // ha a kapcsolat létrejött akkor az alábbi gombok elérhetőek
            document.getElementById("inviteButton").disabled = false;
            document.getElementById("readyButton").disabled = false;

        }).catch((err) => console.error(err.toString()));
});

// Egy játékos hozzáadása
connection.on("AddPlayer", addPlayer);

// Egy játékoz hozzáadása, vagy szerkeztése ha már létezik a játékos rekordja
connection.on("EditOrAddPlayer", editOrAddPlayer);

// kész állapotra váltás
connection.on("PlayerReady", (name) => editPlayer(name, status = "Kész"));

// Meghívó küldése
function SendInvite() {

    let name = document.getElementById("inviteName").value;
    let roomName = document.getElementById('roomName').innerText;

    // a [name] nevű játékos meghívása a [roomname] nevű szobába
    connection.invoke("InvitePlayer", roomName, name)
        .catch((err) => console.error(err.toString()));
}

function addRobotToRoom() {
    let roomName = document.getElementById('roomName').innerText;
    connection.invoke("AddRobot", roomName).catch((err) => console.error(err.toString()));
}

// Kész állapotra váltás
document.getElementById("readyButton").addEventListener("click", function (event) {
    let myName = document.getElementById("name").innerText;
    let roomName = document.getElementById('roomName').innerText;

    connection.invoke("GetReady", roomName, myName).then(() => document.getElementById("startButton").disabled = false)
        .catch(err => console.error(err.toString()));

    event.preventDefault();
});

// A startgomb megnyomásakkor minden játékosnál a játék kezdetét veszi
document.getElementById("startButton")
    .addEventListener("click", (event) => {
        let roomName = document.getElementById('roomName').innerText;

        // Kérés a játék elkezdéséhez
        connection.invoke("StartGame", roomName)
            .catch(err => console.error(err.toString()));
        event.preventDefault();
    });

// A játék elkezdése és a megfelelő oldalra való áttirányítás
connection.on("StartGame", (link) => {
    let myName = document.getElementById("name").innerText;
    window.location = link + "/" + myName;
});

connection.on("Message", (title, type, msg) => {
    VanillaToasts.create({
        title: title,
        text: msg,
        type: type, // success, info, warning, error   / optional parameter
        timeout: 3500 // hide after 5000ms, // optional paremter
    });
});
