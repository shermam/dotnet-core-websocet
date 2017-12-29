angular.module('app')
    .factory('WebSocketService', function ($timeout) {
        var service = {};
        var scheme = document.location.protocol == "https:" ? "wss" : "ws";
        var port = document.location.port ? (":" + document.location.port) : "";
        var url = scheme + "://" + document.location.hostname + port + "/ws";
        var socket = new WebSocket(url);
        var callbacks = [];

        service.onmessage = function(callback){
            callbacks.push(callback);
        };

        service.send = function(data){
            socket.send(data);
        };

        socket.onmessage = function (event) {
            console.log(event);
            angular.forEach(callbacks, function (callback) {
                $timeout(function name() {
                    callback(event.data);
                })
            })
        }

        socket.onclose = function (event) {
            console.log(event);
            
        }

        return service;
    })