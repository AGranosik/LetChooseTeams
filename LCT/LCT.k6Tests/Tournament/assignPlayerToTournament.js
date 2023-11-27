import { check, group } from "k6"
import { assignPlayers, postTournament } from "./common.js";

export const options = {
    scenarios: {
        heavy_load: {
            executor: 'ramping-vus',
            startVUs: 10,
            startTime: 0,
            stages: [
                { duration: '30s', target: 20 },
                { duration: '30s', target: 35 },
                { duration: '1m', target: 35 },
                { duration: '1.5m', target: 0 },
            ],
            gracefulRampDown: '15s',
        },
        real_time_scenario:{
            executor: 'shared-iterations',
            startTime: '250s',
            vus: 30,
            iterations: 230,
            maxDuration: '200s'
        },
    },
    thresholds: {
        http_req_duration: ['p(90) < 6000'],
        http_req_failed: ['rate < 0.02']
    },
}

export default function () {
    const numberOfPlayers = 6
    const postResult = postTournament(numberOfPlayers);
    let id = postResult.body.split('"').join("");
    check(postResult, {
        'is status 200:': (r) => r.status == 200
    });

    if(postResult.status === 200){
        for(var i = 0; i < numberOfPlayers; i++){
            let result = assignPlayers(id);
            check(result, {
                'assign is 200:': (r) => r.status == 200
            })
        }

    }
}