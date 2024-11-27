const client = require("./connection")
const express = require('express')
const app = express()
const bodyParser = require('body-parser')

app.use(bodyParser.json());

app.listen(3000, ()=>{
    console.log("Server is now available and listening to port %d", 3000);
})

client.connect(); 

app.get('/users', (req, res)=>{
    client.query('Select * from users', (err,result)=>{
        if(!err){
            res.send(result.rows);
        }
    });
    client.end;
})

app.get('/users/:id', (req, res)=>{
    client.query(`Select * from users where id=${req.params.id}`, (err,result)=>{
        if(!err){
            res.send(result.rows);
        }
    });
    client.end;
})

app.post('/users', (req, res)=>{
    const user = req.body;
    let insertQuery = `insert into users(id, username, password, email) 
    values('${user.id}','${user.username}','${user.password}','${user.email}')`;

    client.query(insertQuery, (err, result)=>{
        if(!err){
            res.send('Registration Successful');
        } else {
            console.log(err.message);
        }
    })
    client.end;
})

app.put('/users/:id', (req, res)=>{
    const user = req.body;
    let updateQuery = `update users set username = '${user.username}', 
    email = '${user.email}',
    password = '${user.password}'
    where id='${req.params.id}'`;

    client.query(updateQuery, (err, result)=>{
        if(!err){
            res.send('Password Changed Successfully');
        } else {
            console.log(err.message);
        }
    })
    client.end;
})

app.delete('/users/:id', (req, res)=>{
    let deleteQuery = `delete from users where id=${req.params.id}`;
    client.query(deleteQuery, (err,result)=>{
        if(!err){
            res.send(`User deleted successfully`);
        } else {
            console.log(err.message)
        }
    });
    client.end;
})