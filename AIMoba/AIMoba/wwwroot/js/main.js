"use strict"

let canvas;
let context;
let cols = 20;
let rows = 20;
let cellSize;
let endPicture;

// a pálya háttérszíne, a setup függvényben kap értéket
let backgroundColor;

// a későbbi animációkhoz szükséges lehet, az eltelt idő kiszámítását segítti
// TODO: animácíók létrehozása a játékban
let lastTimestamp;

// az amoba tábla (Grid típusu)
let amobaGrid;

let States = {
    "None": 0 ,
    "PlayerOne": 1,
    "PlayerTwo": 2,
    "PlayerThree": 3,
    "PlayerFour": 4 
};

// egy dictionary amely a játékosok alakzataihoz rendeli a színeket
let Colors = {
    1: "red",           // PlayerOne
    2: "blue",          // PlayerTwo
    3: "orange",        // PlayerThree
    4: "darkMagenta"    // PlayerFour 
};

// az oldal betöltésével elindulnak a megjelenítéshez szükséges dolgok 
window.addEventListener('DOMContentLoaded' ,() => {
    setup();
    lastTimestamp = 0;


    canvas.addEventListener('click', handlePlayerInput);

    draw(0);
    //window.requestAnimationFrame(draw);
});

function handlePlayerInput(e) {
    let mousepos = getMousePos(e);
    let iPos = Math.floor(mousepos.y / cellSize);
    let jPos = Math.floor(mousepos.x / cellSize);
    console.log(iPos, jPos);

    let params = window.location.pathname.split('/').filter(x => x.length != 0);
    let gameID = parseInt(params[params.length - 2]);
    let playerID = parseInt(params[params.length - 1]);
    console.log(gameID, playerID);
    fetch('../../makemove', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ GameID: gameID, PlayerID: playerID, Position: { IPos: iPos, JPos: jPos } })
    }).then((res) => {
        console.log(res);
        return res.json();
    }).then(handleResponse);
}

function handleResponse(data) {
    if (data.responsMessage == true) {
        for (let i = 0; i < data.data.length; i++) {
            amobaGrid.fields[data.data[i].pos.iPos][data.data[i].pos.jPos] = data.data[i].mark;
        }
        draw(0);
        if (data.endOfGame) {
            canvas.removeEventListener('click', handlePlayerInput);
            endPicture.onload = function () {
                setTimeout(handleEndOfGame, 1500);
            }    
            if (data.endState == 1) {
                endPicture.src = '../../../images/gamewon.svg';
            } else if (data.endState == -1) {
                endPicture.src = '../../../images/gamelost.svg';
            } else if (data.endState == 0) {
                endPicture.src = '../../../images/gamedraw.svg';
            }
            endPicture.width = canvas.width;
            endPicture.height = canvas.height;
            endPicture.style.backgroundColor = "black";
        }

    } 
}

function handleEndOfGame() {

    canvas.parentElement.appendChild(endPicture);
    canvas.parentElement.removeChild(canvas);
}

function getMousePos(e) {
    var rect = canvas.getBoundingClientRect();
    return {
        x: e.clientX - rect.left,
        y: e.clientY - rect.top
    };
}

// egyszer fut le az draw előtt
function setup(){
    canvas = document.querySelector('#myCanvas');
    context = canvas.getContext('2d');
    cellSize = canvas.width / cols;
    endPicture = new Image();
    // lekerekített sarkok vonal végeknél és vonalok kapcsolódásakor
    context.lineCap = "round";
    context.lineJoin = "round";
    lastTimestamp = 0;

    // színátenetes háttérszín
    backgroundColor = context.createLinearGradient(0, 0, canvas.width, canvas.height);
    backgroundColor.addColorStop(0, "#2193b0");
    backgroundColor.addColorStop(1, "#6dd5ed");;

    // nincs kamera a játékban ezért célszerű aparamétereket úgy megadni hogy:
    // width = canvas szélesség / cellaSpaceing     vagy  cellaSpaceing = canvasSzélessége / width
    // height = canvas magassága / cellaSpaceing    vagy  cellaSpaceing = canvasSzélessége / height
    amobaGrid = new Grid(cols, rows, cellSize, 6);
    
    
    // fillWithDummyData(amobaGrid.fields, amobaGrid.width, amobaGrid.height, 5);
}

// folyamatos frissítés
function draw(timestamp){
    // háttér törlése
    context.fillStyle = backgroundColor;
    context.fillRect(0, 0, canvas.width, canvas.height);
    // a tábla kirajzolása
    amobaGrid.show(context);

    //window.requestAnimationFrame(draw);
}

 // TODO: kitörölni amikor az AImoba fő projectjébe kerül ez a kód
function fillWithDummyData(fields,w,h, range){
    for(let i = 0; i < h; i++){
        for(let j = 0; j < w; j++){
            fields[i][j] = Math.floor(Math.random()*range) 
        }
    }
}
