const Discord = require('discord.js');
const fs = require('fs');
const chartistSvg = require('svg-chartist');
const nodeHtmlToImage = require('node-html-to-image');
const transfer = require('transfer-sh');

const client = new Discord.Client();

const doctors = require('./doctors.json');

client.on('ready', () => {
  console.log(`Logged in as ${client.user.tag}!`);
});


client.on('guildMemberUpdate', async (g0, g1) => {

  var guild = g0.guild;

  await Promise.all([guild.roles.fetch(), guild.members.fetch()]);

  var showRoles = guild.roles.cache.filter(r => doctors.includes(r.name));

  var labels = showRoles.map(r => r.name);
  var values = showRoles.map(r => r.members.size);


  var mcount = guild.roles.cache.filter(r => r.name == "Member").map(r => r.members.size)[0];

  const data = {
    title: 'Soctor Who Membership',
    subtitle: `Total Full Members: ${mcount}`,
    labels,
    series: [
      values
    ]
  }

  const options = {
    width: 700,
    height: 350,
    stackBars: true,
    axisX: {
      showLabel: true,
    },
    axisY: {
      showLabel: true,
      showGrid: true,
      onlyInteger: true
    }
  }

  const opts = {
    options: options,
    title: {
      height: 50,
      fill: "#3c57c3"
    },
    subtitle: {
      height: 25,
      fill: "#808080"
    },
    css: `
    text:not(.ct-label) {
      font-size: 20px;
    }
    .ct-series-a .ct-bar, .ct-series-a .ct-line, .ct-series-a .ct-point/*, .ct-series-a .ct-slice-donut*/{
      stroke: #3c57c3
    }
    .ct-horizontal {
      writing-mode: vertical-lr;
      transform:translateX(10px);
    }
    svg {
      background-color: white;
    }
  `
  }


  var html = await chartistSvg('line', data, opts)
  var buffer = await nodeHtmlToImage({
    html
  })
  fs.writeFileSync("./image.svg", html);
  fs.writeFileSync("./image.png", buffer);
  var url = await (new transfer("./image.png")).upload();

  var ch = await guild.channels.resolve(process.env.CHAN);
  await ch.messages.fetch();
  var ms = ch.messages.cache.filter(msg => msg.author.id === client.user.id);

  const exampleEmbed = new Discord.MessageEmbed()
    .setColor('#3c57c3')
    .setTitle('Member Role Graph')
    .setImage(url)
    .setTimestamp()

  if (ms.size == 0) {
    ch.send(exampleEmbed)
  } else {
    ms.first().edit(exampleEmbed);
  }
});




client.login(process.env.TOKEN);