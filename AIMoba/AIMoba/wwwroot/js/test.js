window.addEventListener('click', () => {
    console.log(window.location.pathname);
    let params = window.location.pathname.split('/').filter(x=>x.length!=0);
    let gameID = parseInt( params[params.length-2]);
    let playerID = parseInt(params[params.length - 1]);
    console.log(gameID, playerID);
    fetch('../../makemove', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ GameID: gameID, PlayerID: playerID, Position: { IPos: 12, JPos: 11 } })
    }).then((res) => {
        console.log(res);
        return res.json();
    }).then((res) => console.log(res));
})

