import { check, } from 'k6';
import { _basePostParams, _baseTournamentApiUrl } from '../variables.js';
import { postTournament } from './common.js';
export const options = {
    scenarios: {
        ramping_vus_scenario:{
            executor: 'ramping-vus',
            startVUs: 0,
            startTime: 0,
            stages: [
                { duration: '30s', target: 50 }, // simulate ramp-up of traffic from 1 to 100 users over 5 minutes.
                { duration: '30s', target: 120 }, // stay at 100 users for 10 minutes
                { duration: '30s', target: 120 }, // stay at 100 users for 10 minutes
                { duration: '30s', target: 0 }, // ramp-down to 0 users
            ],
            gracefulRampDown: '15s',
        },
        shared_scenario:{
            executor: 'shared-iterations',
            startTime: '125s',
            vus: 100,
            iterations: 6000,
            maxDuration: '40s'
        },
        per_vu_iter_scernario:{
            executor: 'per-vu-iterations',
            startTime: '170s',
            vus: 100,
            iterations: 25,
            maxDuration: '55s'
        },
        cosntant_vu_scenario:{
            executor: 'constant-vus',
            startTime: '230s',
            vus: 100,
            duration: '35s',
        }
    },
    thresholds: {
        http_req_duration: [' avg < 2000'],
    },
};


export default function () {
    const result = postTournament();
    check(result, {
        'is status 200:': (r) => r.status == 200
    });
}