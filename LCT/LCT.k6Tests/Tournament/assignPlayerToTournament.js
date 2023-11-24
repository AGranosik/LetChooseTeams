import { check, group } from "k6"
import { assignPlayers, postTournament } from "./common.js";

export const options = {
    stages: [
        { duration: '30s', target: 20 },
        { duration: '30s', target: 35 },
        { duration: '1m', target: 45 },
        { duration: '1.5m', target: 0 },
    ],
    thresholds: {
        http_req_duration: ['p(90) < 4000', 'p(95) < 4800', 'p(99.9) < 6000'],
        http_req_failed: ['rate<0.3']
    },
}

export default function () {
    const numberOfPlayers = 10
    const postResult = postTournament(numberOfPlayers);
    let id = postResult.body.split('"').join("");
    check(postResult, {
        'is status 200:': (r) => r.status == 200
    });
    for(var i = 0; i < numberOfPlayers; i++){
        let result = assignPlayers(id);
        check(result, {
            'assign is 200:': (r) => r.status == 200
        })
    }
}