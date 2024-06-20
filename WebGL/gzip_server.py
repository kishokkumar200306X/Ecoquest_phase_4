import http.server
import socketserver

class GzipHTTPRequestHandler(http.server.SimpleHTTPRequestHandler):
    def end_headers(self):
        if self.path.endswith(".gz"):
            self.send_header("Content-Encoding", "gzip")
        return super(GzipHTTPRequestHandler, self).end_headers()

PORT = 8000

with socketserver.TCPServer(("", PORT), GzipHTTPRequestHandler) as httpd:
    print("serving at port", PORT)
    httpd.serve_forever()
