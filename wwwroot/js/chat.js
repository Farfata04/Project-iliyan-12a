"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    document.getElementById("messageInput").disabled = true;

connection.on("ReceiveMessage", function (result) {
    createIncomingMessage(result);
    scrollToBottom();
});

connection.on('GroupMessage', (message) => {
    var div = document.createElement("div");
    div.classList.add("incoming_msg");
    div.classList.add("group_msg");
    div.innerHTML = `<p>${message}</p>`;

    var msgHistory = document.getElementById("msg_history");

    msgHistory.appendChild(div);
    scrollToBottom();
});


connection.on('SetOnlineStatus', (data) => {
    // get online people by id
    // const peopleList = document.getElementById('online-people');
    
    const user = document.getElementById(data.userId);
    
    // if(data.isOnline)
    // {
    //     user.querySelector('.badge-success').hidden = false;
    //     user.querySelector('.badge-light').hidden = true;
    // }


    user.querySelector('.badge-success').hidden = !data.isOnline;
    user.querySelector('.badge-light').hidden = data.isOnline;
})

connection.start()
    .then(function () {
        document.getElementById("messageInput").disabled = false;
        document.getElementById("messageInput").addEventListener("keyup", handleSendMessage);
    })
    .catch(function (err) {
        return console.error(err.toString());
    });

function handleSendMessage(e) {
    if (e.keyCode !== 13)
        return;

    e.preventDefault();
    const input = document.getElementById("messageInput")
    const message = input.value;

    connection.invoke("SendMessage", message)
        .then(() => createOutgoingMessage(message))
        .catch((err) => console.error(err.toString()))
        .finally(() => {
            input.value = "";

            scrollToBottom();
        });
}

function scrollToBottom() {
    var msgHistory = document.getElementById("msg_history");
    msgHistory.scrollTo(0, msgHistory.scrollHeight);
}

function createOutgoingMessage(message) {
    var div = document.createElement("div");
    div.classList.add("outgoing_msg");

    var p = document.createElement("p");
    p.textContent = message;

    var span = document.createElement("span");
    span.classList.add("time_date");
    span.textContent = new Date().toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true });

    var div2 = document.createElement("div");
    div2.classList.add("sent_msg");

    div2.appendChild(span);
    div2.appendChild(p);
    div.appendChild(div2);

    var msgHistory = document.getElementById("msg_history");
    msgHistory.appendChild(div);

    return div;
}

function createIncomingMessage(message) {
    var div = document.createElement("div");
    div.classList.add("incoming_msg");

    var div2 = document.createElement("div");
    div2.classList.add("incoming_msg_img");

    var img = document.createElement("img");
    img.src = message.profilePicture;
    img.alt = "sunil";

    div2.appendChild(img);

    var div3 = document.createElement("div");
    div3.classList.add("received_msg");

    var div4 = document.createElement("div");
    div4.classList.add("received_withd_msg");

    var p = document.createElement("p");
    p.textContent = message.content;

    var span = document.createElement("span");
    span.classList.add("time_date");

    const dateString =  new Date(message.timeStamp).toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true });
    span.textContent = message.username + " | " + dateString;

    div4.appendChild(span);
    div4.appendChild(p);
    div3.appendChild(div4);
    div.appendChild(div2);
    div.appendChild(div3);

    var msgHistory = document.getElementById("msg_history");
    msgHistory.appendChild(div);

    return div;
}
