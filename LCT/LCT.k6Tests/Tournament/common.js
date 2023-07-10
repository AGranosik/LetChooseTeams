import http from "k6/http";
import { _basePostParams, _baseTournamentApiUrl } from "../variables.js";

export function postTournament(limit = 2) {
    const rand = Date.now() * Math.random()
    const payload = JSON.stringify({
        name: 'performanceTest' + rand.toString(),
        playerLimit: limit
    });

    const result = http.post(_baseTournamentApiUrl + '/create', payload, _basePostParams);
    return result;
}

export function assignPlayers(id){
    const rand = Date.now() * Math.random()
    const payload = JSON.stringify({
        name: 'test' + rand,
        surname: 'performance' + rand,
        tournamentId: id
    });

    const result = http.post(_baseTournamentApiUrl + '/assignPlayer', payload, _basePostParams);
    return result;
}