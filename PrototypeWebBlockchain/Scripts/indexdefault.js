
//$.getScript("./node_modules/web3/dist/web3.min.js", function () {
//    alert("Script loaded but not necessarily executed.");
//});

function getID(id) {
    var editIndex = 1;
    var test = [];
    modifyID = id;
    $("#button").hide();
    $("#update-control").show();
    $("#" + id + "tr").each(function () {
        test = $(this).find("td")
    });

    var ntest = test[0];
    var ttest = ntest['innerHTML'];

    $(test).each(function () {
        if (editIndex == 1) {
            $("#fname").val(this['innerHTML']);
        }

        else if (editIndex == 2) {
            $("#lname").val(this['innerHTML']);

        }

        else if (editIndex == 3) {
            $("#age").val(this['innerHTML']);

        }

        else if (editIndex == 4) {
            $("#contact").val(this['innerHTML']);

        }

        else if (editIndex == 5) {
            $("#address").val(this['innerHTML']);

        }

        editIndex++;
    });
}

function load() {
    $("#loader").show();
    $("#membertable").hide();
}

function loadfinish() {
    $("#loader").hide();
    $("#membertable").show();
}

$("#button3").click(function () {
    $("#update-control").hide();
    $("#button").show();
    $("#fname").val('');
    $("#lname").val('');
    $("#age").val('');
    $("#contact").val('');
    $("#address").val('');
})

//Declare Account to be used
web3.eth.defaultAccount = web3.eth.accounts[0];

web3.personal.unlockAccount(web3.eth.accounts[0], 'password', 1000);

options = {
    gasPrice: '5', // default gasPrice when
    gasLimit: '999999999' // default gasLimit when
}

//Declare the Contract Body
var MemberContract = web3.eth.contract([
	{
	    "constant": false,
	    "inputs": [
			{
			    "name": "_id",
			    "type": "uint256"
			},
			{
			    "name": "_fname",
			    "type": "string"
			},
			{
			    "name": "_lname",
			    "type": "string"
			},
			{
			    "name": "_age",
			    "type": "uint256"
			},
			{
			    "name": "_contactnumber",
			    "type": "string"
			},
			{
			    "name": "_home",
			    "type": "string"
			}
	    ],
	    "name": "UpdateMember",
	    "outputs": [],
	    "payable": false,
	    "stateMutability": "nonpayable",
	    "type": "function"
	},
	{
	    "constant": false,
	    "inputs": [
			{
			    "name": "_fname",
			    "type": "string"
			},
			{
			    "name": "_lname",
			    "type": "string"
			},
			{
			    "name": "_age",
			    "type": "uint256"
			},
			{
			    "name": "_contactnumber",
			    "type": "string"
			},
			{
			    "name": "_home",
			    "type": "string"
			}
	    ],
	    "name": "AddMember",
	    "outputs": [],
	    "payable": false,
	    "stateMutability": "nonpayable",
	    "type": "function"
	},
	{
	    "constant": true,
	    "inputs": [
			{
			    "name": "_number",
			    "type": "uint256"
			}
	    ],
	    "name": "getMember",
	    "outputs": [
			{
			    "name": "",
			    "type": "string"
			},
			{
			    "name": "",
			    "type": "string"
			},
			{
			    "name": "",
			    "type": "uint256"
			},
			{
			    "name": "",
			    "type": "string"
			},
			{
			    "name": "",
			    "type": "string"
			}
	    ],
	    "payable": false,
	    "stateMutability": "view",
	    "type": "function"
	},
	{
	    "constant": true,
	    "inputs": [],
	    "name": "countInstructors",
	    "outputs": [
			{
			    "name": "",
			    "type": "uint256"
			}
	    ],
	    "payable": false,
	    "stateMutability": "view",
	    "type": "function"
	},
	{
	    "constant": false,
	    "inputs": [
			{
			    "name": "_id",
			    "type": "uint256"
			}
	    ],
	    "name": "DeleteMember",
	    "outputs": [],
	    "payable": false,
	    "stateMutability": "nonpayable",
	    "type": "function"
	},
	{
	    "anonymous": false,
	    "inputs": [
			{
			    "indexed": false,
			    "name": "id",
			    "type": "uint256"
			},
			{
			    "indexed": false,
			    "name": "fname",
			    "type": "string"
			},
			{
			    "indexed": false,
			    "name": "lname",
			    "type": "string"
			},
			{
			    "indexed": false,
			    "name": "age",
			    "type": "uint256"
			},
			{
			    "indexed": false,
			    "name": "contactnumber",
			    "type": "string"
			},
			{
			    "indexed": false,
			    "name": "home",
			    "type": "string"
			},
			{
			    "indexed": false,
			    "name": "proc",
			    "type": "string"
			}
	    ],
	    "name": "MemberEvent",
	    "type": "event"
	}
]);

//Declare the contract Address
var contract = MemberContract.at('0xd07eb3e85f791a12aF158271bbFdA3e6F708D722');

var count = contract.countInstructors();

//for (i = 1; i <= count; i++) {
//    var result = contract.getMember(i);
//    if (result[0] != '') {
//        $("#membertable").append('<tr id="' + i + 'tr"><td>' + result[0] + '</td>' + '<td>' + result[1] + '</td>' + '<td>' + result[2] + '</td>' + '<td>' + result[3] + '</td>' + '<td>' + result[4] + '</td> <td> <button type="button" class="btn btn-warning" id="' + i + '" onclick="getID(this.id)">EDIT</button> <button type="button" class="btn btn-danger" id="' + i + '" onclick="Delete(this.id)">Delete</button> </td> </tr>');
//    }
//}

$("#button").click(function () {
    load();
    contract.AddMember($("#fname").val(), $("#lname").val(), $("#age").val(), $("#contact").val(), $("#address").val(), { gas: 999999 })
});

$("#button2").click(function () {
    load();
    contract.UpdateMember(modifyID, $("#fname").val(), $("#lname").val(), $("#age").val(), $("#contact").val(), $("#address").val(), { gas: 999999 });
});

function Delete(id) {
    load();
    contract.DeleteMember(id, { gas: 999999 });
}

var jsonFile = new Array();

var _id = transaction.id;
var _member_id = transaction.member_id;
var _filename = transaction.filename;
var _transactionhash = transaction.transactionHash;
var _filepath = transaction.filepath;
var _date = transaction.date

jsonFile.push(new Object({
    id : Int32.Parse(imageId),
    member_id : Int32.Parse(Session["ID"].ToString()),
    filename : imageName,
    filepath : imagepath,
    date : DateTime.Now.ToString()
}));


var FileUploadEvent = ImageContract.FileUploadEvent();
FileUploadEvent.watch(function (error, result) {
    if (!error) {
        $.ajax({
            url: 'AddTransactionToTable',
            type: 'POST',
            data: { "data": result['transactionHash'] },
            success: function (JTransaction) {
            }
        });
    }
});