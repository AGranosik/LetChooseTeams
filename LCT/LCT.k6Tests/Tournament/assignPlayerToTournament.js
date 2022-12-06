import { check, group } from "k6"
import { assignPlayers, postTournament } from "./common.js";

export const options = {
    stages: [
        { duration: '30s', target: 50 },
        { duration: '30s', target: 50 }, // simulate ramp-up of traffic from 1 to 100 users over 5 minutes.
        { duration: '1m', target: 100 }, // stay at 100 users for 10 minutes
        { duration: '30s', target: 0 }, // ramp-down to 0 users
    ],
    thresholds: {
        http_req_duration: ['p(90) < 4000', 'p(95) < 4800', 'p(99.9) < 6000'],
    },
}

export default function () {
    const postResult = postTournament(20);
    let id = postResult.body.split('"').join("");
    check(postResult, {
        'is status 200:': (r) => r.status == 200
    });
    for(var i = 0; i < 20; i++){
        let result = assignPlayers(id);
        check(result, {
            'assign is 200:': (r) => r.status == 200
        })
    }

}