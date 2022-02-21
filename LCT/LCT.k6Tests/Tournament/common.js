import http from "k6/http";
import { _basePostParams, _baseTournamentApiUrl } from "../variables.js";

export function postTournament() {
    const rand = Date.now() * Math.random()
    const payload = JSON.stringify({
        name: 'performance test' + rand.toString(),
        playerLimit: 2
    });

    const result = http.post(_baseTournamentApiUrl + '/create', payload, _basePostParams);
    return result;
}