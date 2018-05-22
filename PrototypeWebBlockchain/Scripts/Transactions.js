$.ajax({
    url: "ValidateImages",
    type: "POST",
    success: function (JTransaction) {
        if (JTransaction)

            $('#loader').show();

            for (var i = 0; i < JTransaction["JTransaction"].length; i++) {

                var token = $('#TokenForm input[name=__RequestVerificationToken]').val();

                if (JTransaction["JTransaction"][i]["status"] == "Status/cross.png") {
                    $("#transactiontable")
                        .append(
                        "<tr><td>" + JTransaction["JTransaction"][i]["id"] +
                        "</td><td>" + JTransaction["JTransaction"][i]["filename"] +
                        "</td> <td>  <img src=/images/" + JTransaction["JTransaction"][i]["status"] + " style='width:50px; height:50px'></td> <td>" +
                        JTransaction["JTransaction"][i]["date"] + "</td>" +
                        "<td style='width:250px; text-align:center;'><form method='post' id='' action='#'> <input type='hidden' name='__RequestVerificationToken' value='" + token + "'><input type='hidden' name='id' value='" + JTransaction["JTransaction"][i]["backuphash"] + "'> <input type='submit' class='btn btn-warning' value='Restore File'> </form></td></tr>"
                        );
                }
                else {
                    $("#transactiontable")
                        .append(
                        "<tr><td>" + JTransaction["JTransaction"][i]["id"] +
                        "</td><td>" + JTransaction["JTransaction"][i]["filename"] +
                        "</td> <td>  <img src=/images/" + JTransaction["JTransaction"][i]["status"] + " style='width:50px; height:50px'> </td> <td>" +
                        JTransaction["JTransaction"][i]["date"] + "</td> </tr>"
                        );
                }

            }
                $('#transactiontable').show();
                $('#loader').hide();
    }
});
