"use strict"

let canvas;
let context;
let cols = 20;
let rows = 20;
let cellSize;

// a pálya háttérszíne, a setup függvényben kap értéket
let backgroundColor;

// a későbbi animációkhoz szükséges lehet, az eltelt idő kiszámítását segítti
// TODO: animácíók létrehozása a játékban
let lastTimestamp;

// az amoba tábla (Grid típusu)
let amobaGrid;

let States = {
    "None": 0,
    "PlayerOne": 1,
    "PlayerTwo": 2,
    "PlayerThree": 3,
    "PlayerFour": 4
};

// egy dictionary amely a játékosok alakzataihoz rendeli a színeket
let Colors = {
    1: "red",           // PlayerOne
    2: "blue",          // PlayerTwo
    3: "#e4ed85",        // PlayerThree
    4: "darkMagenta"    // PlayerFour 
};

// az oldal betöltésével elindulnak a megjelenítéshez szükséges dolgok 
window.addEventListener('DOMContentLoaded', () => {
    setup();
    lastTimestamp = 0;

    draw(0);
    //window.requestAnimationFrame(draw);
});

function getMousePos(e) {
    var rect = canvas.getBoundingClientRect();
    return {
        x: e.clientX - rect.left,
        y: e.clientY - rect.top
    };
}

// egyszer fut le az draw előtt
function setup() {
    canvas = document.querySelector('#myCanvas');
    context = canvas.getContext('2d');
    cellSize = canvas.width / cols;

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

}

// folyamatos frissítés
function draw(timestamp) {
    // háttér törlése
    context.fillStyle = backgroundColor;
    context.fillRect(0, 0, canvas.width, canvas.height);
    // a tábla kirajzolása
    amobaGrid.show(context);

    //window.requestAnimationFrame(draw);
}
