<?php
	readfile($_SERVER['DOCUMENT_ROOT'] . parse_url($_SERVER['REQUEST_URI'], PHP_URL_PATH));// or http_response_code(503);
?>