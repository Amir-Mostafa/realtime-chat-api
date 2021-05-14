using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using realtime.Models;
namespace realtime
{

    [HubName("chat")]
    public class chatHub:Hub
    {

        chatContext db;
        public chatHub(chatContext db)
        {
            this.db = db;
        }
        public void tellTyping(int sId,int rID)
        {
            List<Connection> all = db.Connections.Where(n => n.SendId == sId&&n.UserId==rID).ToList();
            for(int i=0;i<all.Count;i++)
            {
                Clients.Client(all[i].ConId).SendAsync("typing");
            }
        }
        public void tellNotTyping(int sId,int rID)
        {
            List<Connection> all = db.Connections.Where(n => n.SendId == sId && n.UserId == rID).ToList();
            for (int i = 0; i < all.Count; i++)
            {
                Clients.Client(all[i].ConId).SendAsync("nottyping");
            }
        }
        public void sendmessage(int sid,int rid,string name,string rName,string msg)
        {
            Message m = new Message()
            {
                Msg = msg,
                Name = name,
                SenderId = sid,
                ReceverId = rid,
                Date = DateTime.Now,
                Status = 0,
                Rname = rName
            };
            db.Messages.Add(m);
            
            //save data

            
            List<Connection> c = db.Connections.Where(n => (n.UserId == rid&&n.SendId==sid)||(n.UserId==rid&&n.SendId==0)).ToList();
            for(int i=0;i<c.Count;i++)
            {
                Clients.Client(c[i].ConId).SendAsync("recive", name, msg, DateTime.Now.ToString());
                
            }
             c = db.Connections.Where(n => n.UserId == sid && n.SendId == rid).ToList();
            for(int i=0;i<c.Count;i++)
            {
                Clients.Client(c[i].ConId).SendAsync("AllSend", name, msg, DateTime.Now.ToString());

            }
            db.SaveChanges();
        }
        public void saveData(int id,int sID,string conID)
        {
            Connection c = new Connection()
            {
                ConId = conID,
                UserId = id,
                SendId=sID
            };
            db.Connections.Add(c);
            string on = "off";
            if (db.Connections.Where(n => n.UserId == sID).FirstOrDefault() != null)
                on = "on";
            List<Message> last = db.Messages.Where(n => (n.SenderId == sID && n.ReceverId == id)||(n.SenderId == id && n.ReceverId == sID)).OrderBy(n=>n.Date).ToList();
            Clients.Client(conID).SendAsync("sendMessages",last);
            if (sID != 0)
            {
                Clients.Client(conID).SendAsync("status", on);

                List<Message> unread = db.Messages.Where(n => n.ReceverId == id &&n.SenderId==sID&& n.Status == 0).ToList();
                for(int i=0;i<unread.Count;i++)
                {
                    unread[i].Status = 1;
                    db.Entry(unread[i]).State = EntityState.Modified;
                }
                db.SaveChanges();


            }
            db.SaveChanges();

            //send to all connection 
            List<Connection> all = db.Connections.Where(n => n.SendId == c.UserId).ToList();
            for (int i = 0; i < all.Count; i++)
            {
                Clients.Client(all[i].ConId).SendAsync("status", "on");
            }
        }
        public void getChat(int id, int sID, string conID)
        {
            List<Message> last = db.Messages.Where(n => (n.SenderId == sID && n.ReceverId == id) || (n.SenderId == id && n.ReceverId == sID)).OrderBy(n => n.Date).ToList();
            Clients.Client(conID).SendAsync("sendMessages", last);

        }

        public void getUnReadMessages(int id,string conID)
        {
            List<Message> all = db.Messages.Where(n => n.ReceverId == id && n.Status == 0).ToList();
            Clients.Client(conID).SendAsync("UnRead", all);
            //List<Message> all = db.Messages.Where(n => (n.ReceverId == id || n.SenderId == id)).OrderByDescending(n => n.Date).ToList();
            //all = all.GroupBy(x => x.Name).Select(n => n.First()).ToList();

            //List<string> all = db.Messages.GroupBy(n=>n.Name).Where(n=>n.Where(g=>g.SenderId==id)).Select(p => p.Name).Distinct().ToList();
            //Clients.Client(conID).SendAsync("UnRead",all);
            

        }
        public void getLastChat(int id,string conID)
        {
            //List<Message> all = db.Messages.Where(n => (n.ReceverId == id || n.SenderId == id)).OrderByDescending(n => n.Date).ToList();
            //List<Message> all1 = all.GroupBy(x => x.Name).Select(n => n.First()).ToList();
            //List<Message> all2 = all.GroupBy(x => x.Rname).Select(n => n.First()).ToList();
            //all1.AddRange(all2);
            //Clients.Client(conID).SendAsync("lastChat", all1.Distinct());
            List<string> names = new List<string>();
            List<Message> all = db.Messages.Where(n => (n.ReceverId == id || n.SenderId == id)).OrderByDescending(n => n.Date).ToList();

            List<Message> send = new List<Message>();

            List<Message> receve = new List<Message>();
            for (int i=0;i<all.Count;i++)
            {
                if (names.IndexOf(all[i].Name) == -1)
                {
                    names.Add(all[i].Name);
                    send.Add(all[i]);
                }
                if (names.IndexOf(all[i].Rname) == -1)
                {
                    names.Add(all[i].Rname);
                    receve.Add(all[i]);
                }
            }
            
            Clients.Client(conID).SendAsync("lastChat", send,receve);
        }
        public void checkOnline(int id,string conID)
        {
            string on = "off";
            if (db.Connections.Where(n => n.UserId == id).FirstOrDefault() != null)
                on = "on";
            Clients.Client(conID).SendAsync("status", on);
        }
        public override Task OnConnectedAsync()
        {
            string id = Context.ConnectionId;
            Clients.Client(id).SendAsync("sendID", id);

            return base.OnConnectedAsync();
        }
        
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Connection c = db.Connections.Where(n => n.ConId == Context.ConnectionId.ToString()).FirstOrDefault();

            List<Connection> all = db.Connections.Where(n => n.SendId == c.UserId).ToList();
            for(int i=0;i<all.Count;i++)
            {
                Clients.Client(all[i].ConId).SendAsync("status", "off");
            }
            //string on = "off";

            //if (db.Connections.Where(n => n.UserId == c.UserId).ToList().Count() <=1)
            //    Clients.All.SendAsync("status", on);
            db.Connections.Remove(c);

            db.SaveChanges();
            return base.OnDisconnectedAsync(exception);
        }
        
    }
}
