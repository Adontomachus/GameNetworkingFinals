const client = require("./connection")
const express = require('express')
const jwt = require('jsonwebtoken')
const app = express()
const bodyParser = require('body-parser')

app.use(bodyParser.json());

app.listen(3000, ()=>{
    console.log("Server is now available and listening to port %d", 3000);
})

client.connect(); 

const JWT_SECRET = 'this-is-a-secret-token'


const authenticateToken = (req, res, next) => {
    const authHeader = req.headers['authorization'];
    const token = authHeader && authHeader.split(' ')[1];

    if(!token){
        return res.status(401).send('Access token required');        
    }

    jwt.verify(token, JWT_SECRET, (err, user)=>{
        if(err){
            return res.status(403).send('Invalid or expired token');
        } else {
            req.user = user;
            next();
        }
    });
}



app.get('/users', (req, res)=>{
    client.query('Select * from users', (err,result)=>{
        if(!err){
            res.send(result.rows);
        } 
    });
    client.end;
})

//FOR LEADERBOARDS
app.get('/players', (req, res) => {
    client.query('SELECT * FROM players ORDER by kills DESC LIMIT 10', (err, result) => {
        if (err) {
            console.log(err.message);
            return res.status(500).send('Server error');
        }

        res.send(result.rows);  // Return the rows of the players
    });
});

app.get('/players/:playername', (req, res) => {
    const playername = req.params.playername;
    
    client.query('SELECT * FROM players WHERE playername = $1', [playername], (err, result) => {
        if (err) {
            console.log(err.message);
            return res.status(500).send('Server error');
        }

        if (result.rows.length === 0) {
            return res.status(404).send('Player not found');
        }

        res.send(result.rows); 
    });
});

//FOR LEADERBOARDS
app.get('/users/:id', (req, res)=>{
    client.query(`Select * from users where id =${req.params.id}`, (err,result)=>{
        if(!err){
            res.send(result.rows);
        }
    });
    client.end;
})

app.post('/users', (req, res)=> {
    const user = req.body;
    let insertQuery = `insert into users(id, email, username, password) 
                       values('${user.id}', '${user.email}', '${user.username}', '${user.password}')`;

    client.query(insertQuery, (err, result)=>{
        if(!err){
            res.send('Registration was successful')
        }
        else{ console.log(err.message) }
    })
    client.end;
})


app.put('/users/:id', (req, res)=> {
    let user = req.body;
    let updateQuery = `update users set username='${user.username}', password='${user.password}' where id = ${req.params.id}`

    client.query(updateQuery, (err, result)=>{
        if(!err){
            res.send('Update was successful')
        }
        else{ console.log(err.message) }
    })
    client.end;
})

app.delete('/users/:id', (req, res)=> {
    let insertQuery = `delete from users where id=${req.params.id}`

    client.query(insertQuery, (err, result)=>{
        if(!err){
            res.send('Deletion was successful')
        }
        else{ console.log(err.message) }
    })
    client.end;
})

app.delete('/players/:playername', (req, res) => {
    const playername = req.params.playername;
    
    client.query('DELETE FROM players WHERE playername = $1 returning *', [playername], (err, result) => {
        if (err) {
            console.log(err.message);
            return res.status(500).send('Server error');
        }

        if (result.rows.length === 0) {
            return res.status(404).send('Player not found');
        }

        res.send(result.rows); 
    });
});

app.post('/register', (req,res)=>{
    try {
        const {username, email, password} = req.body;

        if(!email || !username || !password)
            return res.send('Missing one or more fields');

        client.query(`SELECT * from users where username = '${username}'`, (err, result)=>{
            if(err){
                console.log(err.message);
            } else {
                if(result.rows.length > 0){
                    return res.send('User already exists');
                }
            }
        });

        let insertQuery = `insert into users(username, email, password) values('${username}', '${email}', '${password}')`;
        client.query(insertQuery, (err,result)=>{
                if(!err){
                    res.send('User registered successfully')
                } else {
                    console.log(err.message)
                }
            });

    } catch (error){
        console.error('Registration error:', error);
        res.sendStatus(500).json({ error: 'Server error'});
    }
})

app.post('/login', (req,res)=> {
     try {
        const { username, password } = req.body;
        if(!password || !username){
            return res.send('Missing fields');
        }

        let passwordQuery = `SELECT * from users where username = '${username}' AND password = '${password}'`;
        client.query(passwordQuery, (err, result)=>{
            if(!err){
                if(result.rows === 0){
                    return res.status(401).send('Invalid Username or Password');
                } else {
                    const user = result.rows[0];
                    const token = jwt.sign( {userId: user.username, email: user.email}, JWT_SECRET, {expiresIn: '24h'});
                    res.json({token});
                }
            }
        });
     } catch (error){
        console.error('Login Error', error);
     }

})

app.post('/playerlogin', (req,res)=> {
    try {
       const { playername, password } = req.body;
       if(!password || !playername){
           return res.send('Missing fields');
       }

       let passwordQuery = `SELECT * from players where playername = '${playername}' AND password = '${password}'`;
       client.query(passwordQuery, (err, result)=>{
           if(!err){
               if(result.rows === 0){
                   return res.status(401).send('Invalid Username or Password');
               } else {
                   const user = result.rows[0];
                   const token = jwt.sign( {playerId: user.playername, password: user.password}, JWT_SECRET, {expiresIn: '24h'});
                   res.json({token});
               }
           }
       });
    } catch (error){
       console.error('Login Error', error);
    }

})

app.post('/players', (req, res)=> {
    const user = req.body;
    let insertQuery = `insert into players(playername, password, kills, deaths) 
                       values('${user.playername}', '${user.password}', '${user.kills}', '${user.deaths}')`;

    client.query(insertQuery, (err, result)=>{
        if(!err){
            res.send('Registration was successful')
        }
        else{ console.log(err.message) }
    })
    client.end;
})



  