import time
from locust import HttpUser, task, between


username = 'NXESIVWEVUUOLSMRIV@TPWLB.COM'
api_key = '9e22bcee-7b9b-449b-a6c4-287b93f63822'
message = 'Hello Locust!'

class ApiLoadTest(HttpUser):
    wait_time = between(1, 5)
    @task(1)
    def post_to_api(self):
        self.client.post(f"/api/YaspiMessage?key={api_key}&message={message}&username={username}", name="/api")
        time.sleep(1)

    def on_start(self):
        self.client.verify = False
        time.sleep(1)

