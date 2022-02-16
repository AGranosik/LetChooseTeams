import { check, sleep } from "k6";
import http from "k6/http";
import { _basePostParams, _baseTournamentApiUrl } from "../variables.js";

export const options = {
    stages: [
        { duration: '1m', target: 100 }, // simulate ramp-up of traffic from 1 to 100 users over 5 minutes.
        { duration: '2m', target: 100 }, // stay at 100 users for 10 minutes
        { duration: '1m', target: 0 }, // ramp-down to 0 users
    ],
    thresholds: {
        http_req_duration: ['p(99)<200'], // 99% of requests should be below 400ms
    },
}

export default function () {
    var id = addTournamentToGet();
    const result = http.get(_baseTournamentApiUrl + '/' + id);
    check(result, {
        'is status 200:': (r) => r.status == 200
    });
}

function addTournamentToGet() {
    const payload = JSON.stringify({
        name: 'performance test',
        playerLimit: 2
    });

    const result = http.post(_baseTournamentApiUrl + '/create', payload, _basePostParams);
    check(result, {
        'is status 200:': (r) => r.status == 200
    });
    return result.body;
}