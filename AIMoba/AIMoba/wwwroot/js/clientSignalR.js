"use strict"

let connection = new signalR.HubConnectionBuilder()
    .withUrl("/gameHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

let endPicture = new Image();

connection.start().then(function () {
    let roomName = document.getElementById("roomName").innerText;
    let myname = document.getElementById("name").innerText;

    console.log("feliratkozas");
    connection.invoke("Subscribe", roomName, myname).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("myCanvas").addEventListener('click', handlePlayerInput);

function handlePlayerInput(e) {
    let roomName = document.getElementById("roomName").innerText;

    let mousepos = getMousePos(e);
    let iPos = Math.floor(mousepos.y / cellSize);
    let jPos = Math.floor(mousepos.x / cellSize);

    connection.invoke("Move", roomName, { IPos: iPos, JPos: jPos });
}

connection.on("WaitForMove", (pos, mark) => {
    console.log(pos, mark);
    amobaGrid.fields[pos.iPos][pos.jPos] = mark;
    draw(0);
});

connection.on("GameEnded", (name) => {
    let myname = document.getElementById("name").innerText;
    document.getElementById("backButton").style.visibility = "visible";

    canvas.removeEventListener('click', handlePlayerInput);

    endPicture.onload = function () {
        setTimeout(() => {
            canvas.parentElement.appendChild(endPicture);
            canvas.parentElement.removeChild(canvas);
        }, 1500);
    }

    if (name == myname) {
        endPicture.src = '/images/gamewon.png';
    } else {
        endPicture.src = '/images/gamelost.png';
    }

    endPicture.width = canvas.width;
    endPicture.height = canvas.height;

});

connection.on("Message", (title, type, msg) => {
    VanillaToasts.create({
        title: title,
        text: msg,
        type: type, // success, info, warning, error   / optional parameter
        timeout: 3500
    });
});