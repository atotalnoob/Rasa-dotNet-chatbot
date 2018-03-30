$(document).ready(function () {



    $('textarea').keypress( //This part gets the input from the input field and assigns it to msg variable

        function (e) {

            if (e.keyCode === 13) { //keycode 13 is enter

                e.preventDefault();

                var msg = $(this).val();

                var conversationID = $("#conversationID").attr("conversationID");

                $(this).val('');

                if (msg !== '') {

                    $('<div class="msg_b">' + msg + '</div>').insertBefore('.msg_push'); //user message posted

                    $('.msg_body').scrollTop($('.msg_body')[0].scrollHeight); //scrolls chatbox



                    $.ajax({ //calling controller to update the chat

                        type: "POST",

                        url: "/home/chat",

                        data: { userInput: msg, conversationID: conversationID },

                        action: {},

                        beforeSend: function () {



                            $('<div class="msg_a" id="botTyping">' + '...' + '</div>').insertBefore('.msg_push').show(); //show bot is typing

                        },

                        success: function (data) {



                            $('#botTyping').remove(); //hide bot is

                            if (data !== '')

                                $('<div class="msg_a">' + data + '</div>').insertBefore('.msg_push'); //bot message posted

                            $('.msg_body').scrollTop($('.msg_body')[0].scrollHeight); //scrolls chatbox



                            //alert(data); //was used as a check, but not really needed any longer.

                        },



                        error: function () {

                            $('#botTyping').remove(); //hide bot is

                            $('<div class="msg_a">I am sorry, it seems I ran into a problem!</div>').insertBefore('.msg_push'); //bot message posted

                            $('.msg_body').scrollTop($('.msg_body')[0].scrollHeight); //scrolls chatbox



                            //alert("error"); //shows error, if happens.

                            //If an error comes up here, its probably a problem connecting to API.AI.

                        }

                    });

                }







            }



        });



});





function doSomething() {

    $('#chat-input').sendkeys("this will be simulated");

}

