angular.module('app')
    .controller('MainController', function (WebSocketService) {
        
        var ctrl = this;

        ctrl.send = function (ev, message) {
            if (ev.key !== 'Enter') {
                return;
            }

            WebSocketService.send(message);
            
        }

        ctrl.messages = [];

        WebSocketService.onmessage(function (message) {
            ctrl.messages.push(message);
        })

    })