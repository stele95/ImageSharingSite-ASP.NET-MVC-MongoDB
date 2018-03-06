function GetProductList() {
    $.ajax(
        {
            type: "GET", 
            url: "/Home/GetImages",
            success: function (returnValue) //On Successfull service call
            {
                GetProductListSucceeded(returnValue);
            },
            error: function () // When Service call fails
            {
                alert("Error loading products" + result.status + " " + result.statusText);
            }
        }
    );
}

function GetProductListSucceeded(result) {

    var arr_from_json = JSON.parse(result);

    
    var parent = document.getElementById("slicke");

    for (i = 0; i < arr_from_json.length; i++) {
        try {
            var product = eval('(' + arr_from_json[i] + ')');
        }
        catch (exception) {
            return;
        }

        var productContainer = document.createElement("div");
        $(productContainer).attr("class","slikecontainer");

        var productItem = document.createElement("li");
        $(productItem).attr("class", "listslika");        

        $(productItem).attr("id", i + 1);

        var productImage = document.createElement("img");
        $(productImage).attr("src", 'data:image/png;base64,' + product.Slika);
        $(productImage).attr("class", "slikezaprikaz");

        var productName = document.createElement("label");        
        $(productName).attr("class", "labelklasa");
        productName.innerHTML = product.Name;      

 
        productContainer.appendChild(productImage);

        productItem.appendChild(productContainer);
        productItem.appendChild(productName);

        
        parent.appendChild(productItem);
    }

    

}


$(document).ready(function () {

    var loc = window.location.href;    
    if (loc == "http://localhost:6550/User?register=true") {
        console.log("?register=true");
        $('#register-form').toggleClass('show-form');
        $('#login').animate({
            right: "-1000px"
        }, 500, "swing");
    } else if (loc == "http://localhost:6550/User?login=true") {
        $('#register-form').css({
            marginLeft: '-500px'
        });
        $('#login').animate({
            right: "0px"
        }, 500, "swing");
    }
    else if (loc == "http://localhost:6550/") {
        GetProductList();
    }

    $('#signup').on('click', function () {
        console.log("signup");
        $('#register-form').toggleClass('show-form');
        $('.show-form').css({
            marginLeft: '897px !important'
        });
        $('#login').animate({
            right: "-1000px"
        }, 500, "swing");
    });

    $('#signin').on('click', function () {
        console.log("signin");
        $('#register-form').toggleClass('show-form');
        $('#register-form').css({
            marginLeft: '-500px'
        });
        $('#login').animate({
            right: "0px"
        }, 500, "swing");
    });


   
    //form processing - login
    $('#login-form').submit(function (event) {       

        var data = {
            'username': $('input[name="username"]').val(),
            'password': $('input[name="password"').val()
        };
             

        $.ajax({
            type: 'POST',
            url: '/User/Login',
            data: data,
            dataType: 'json',
            encode: true
        }).done(function (returnValue) {            
            alert(returnValue.message);
            if (returnValue.isok)
                window.location.replace("/");
            });        
        event.preventDefault();
    });

 
});




