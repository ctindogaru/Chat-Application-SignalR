var userName = prompt("Enter your name: ");
var chat = $.connection.chatHub;

chat.client.messageReceived = function (originatorUser, message, time) {
    $("#messages").append('<li>' + time + ' ' + '<strong>' + originatorUser + '</strong>: ' + message);
};

chat.client.lastMessages = function (messages) {
    $("#messages").empty();
    for (var i = messages.length-1; i >= 0; i--)
        $("#messages").append('<li>' + messages[i].Date + ' ,' + messages[i].Time + ' ' + '<strong>' + messages[i].Username + '</strong>: ' + messages[i].MessageText);
};

chat.client.getConnectedUsers = function (userList) {
    $("#userList").empty();
    for (var i = 0; i < userList.length; i++)
        $("#userList").append('<li>' + userList[i] + '</li>');
};

chat.client.newUserAdded = function (newUser) {
    $("#userList").append('<li>' + newUser + '</li>');
}

$("#messageBox").focus();

$("#sendMessage").click(function () {
    chat.server.send(userName, $("#messageBox").val());
    $("#messageBox").val("");
    $("#messageBox").focus();
});

$("#messageBox").keyup(function (event) {
    if (event.keyCode == 13)
        $("#sendMessage").click();
});

$.connection.hub.logging = true;
$.connection.hub.start().done(function () {
    chat.server.connect(userName);
});
