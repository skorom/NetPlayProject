class Grid{

    constructor(width = 20, height = 20, cellSize=10, cellPadding = 7){
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        // a cellák széle és a bennük lévő szimbólum közötti hely
        this.cellPadding = cellPadding; 

        // a tábla reprezentciója kétdimenziós tömb ként
        this.fields = new Array(height);
        
        for(let i = 0; i < this.height; i++){
            this.fields[i] = new Array(this.width);
            for(let j = 0; j < this.width; j++){
                this.fields[i][j] = States["None"];
            }
        }
        this.loadSymbols();

    }

    show(context){
        context.lineWidth = 2
        for(let i = 0; i < this.height; i++){
            for(let j = 0; j < this.width; j++){
                if(this.fields[i][j] !== States["None"]){
                    // a canvas állapotána mentése (a translate miatt a relatív posizió megváltozik)
                    context.save();
                    // relatív pozició áttálíttása
                    context.translate(j*this.cellSize, i*this.cellSize);
                    // szín beállítása
                    context.strokeStyle = Colors[this.fields[i][j]];
                    // alakzat kirajzolása
                    context.stroke(this.symbols[this.fields[i][j]]);
                    // a canvas beállításainak visszaálítása
                    context.restore();
                }
            }
        }

        // a négyzetrács kirajzolása
        context.strokeStyle = "black";
        context.lineWidth = 2;
        for(let i = 1; i < this.height; i++){
            context.beginPath();
            context.moveTo(0, i*this.cellSize);
            context.lineTo(this.width*this.cellSize, i*this.cellSize);
            context.stroke();
        }

        for(let i = 1; i < this.width; i++){
            context.beginPath();
            context.moveTo(i*this.cellSize, 0);
            context.lineTo(i*this.cellSize, this.height*this.cellSize);
            context.stroke();
        }
        
        // a keret kirajzolása
        // Note: a vonal fele nem látszik a canvason (mivel a vonalat a canvas szélére rajzolja)
        // ezért a vastagság kétszer akkora mint a belső négyzethálóé 
        context.lineWidth *= 2;
        context.strokeRect(0, 0, this.width*this.cellSize, this.height*this.cellSize);
       
    }

    /*
        (left,top)    |  (center,top)     |   (right,top)
        ----------------
        (left,middle) |  (center,middle)  |   (right,middle)
        ----------------
        (left,bottom) |  (center,bottom)  |   (right,bottom)

    */
    // a szimbólumok betöltése (nem feltétlenül végleges, szabadon kiegészíthető)
    loadSymbols(){
        // X
        let symbolX = new Path2D();
            symbolX.moveTo(this.cellPadding, this.cellPadding);                     // left top
            symbolX.lineTo(this.cellSize - this.cellPadding,                        // right bottom
                this.cellSize - this.cellPadding);
            symbolX.moveTo(this.cellPadding, this.cellSize - this.cellPadding);     // left bottom
            symbolX.lineTo(this.cellSize - this.cellPadding, this.cellPadding);     // right top

        // kör
        let symbolO = new Path2D();
            symbolO.arc(this.cellSize/2, this.cellSize/2,               // egy teljes kör 0 tól 2PI-ig 
                this.cellSize/2 - this.cellPadding, 0, Math.PI * 2);   
                
        // háromszög
        let symbolTriangle = new Path2D();
            symbolTriangle.moveTo(this.cellSize/2, this.cellPadding);                   // center top
            symbolTriangle.lineTo(this.cellSize - this.cellPadding,                     // right bottom
                this.cellSize - this.cellPadding);
            symbolTriangle.lineTo(this.cellPadding, this.cellSize - this.cellPadding);  // left bottom
            symbolTriangle.lineTo(this.cellSize/2, this.cellPadding);                   // center top

        // egy sarkára fordított négyet
        let symbolQuad = new Path2D();
            symbolQuad.moveTo(this.cellSize/2, this.cellPadding);                   // center top 
            symbolQuad.lineTo(this.cellSize - this.cellPadding, this.cellSize/2);   // right middle 
            symbolQuad.lineTo(this.cellSize/2, this.cellSize - this.cellPadding);   // center buttom  
            symbolQuad.lineTo(this.cellPadding, this.cellSize/2);                   // left middle
            symbolQuad.lineTo(this.cellSize/2, this.cellPadding);                   // center top

        // egy dictionary amely a játékosokhoz rendeli az alakzatokat
        this.symbols = {
            2: symbolO,           // PlayerTwo
            1: symbolX,           // PlayerOne
            3: symbolTriangle,    // PlayerThree
            4: symbolQuad         // PlayerFour 
        };

    }

}