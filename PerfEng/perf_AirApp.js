import http from 'k6/http';
import { check, sleep } from 'k6';

const token = __ENV.API_TOKEN;

export const options = {
  stages: [
    { duration: '1m', target: 20 },  // Ramp-up to 20 users
    { duration: '3m', target: 100 }, // Stress phase: ramp up to 100 users
    { duration: '1m', target: 0 },   // Scale down to 0 users
  ],
};

export default function () {
  const url = 'https://your-api-endpoint.com/resource';

  const params = {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
  };

  const payload = JSON.stringify({
    name: 'New Resource',
    status: 'active',
  });
  // Replace with your actual API endpoint URL
  const res = http.post(url, payload, params); 
  
  // Validate that the system responds correctly
  check(res, { 'status was 200': (r) => r.status == 200 });
  
  sleep(1); // Wait 1 second between requests per user
}
