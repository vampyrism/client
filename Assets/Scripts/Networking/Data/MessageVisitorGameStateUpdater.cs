using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Server
{
    class MessageVisitorGameStateUpdater : IMessageVisitor
    {
        public void Visit(MovementMessage m)
        {
            //Debug.Log(m);


            GameManager.instance.TaskQueue.Enqueue(new Action(() => {
                GameManager.instance.Entities.TryGetValue(m.GetEntityId(), out Entity e);
                //Player p = (Player)e.gameObject.GetComponent<Player>();

                /*if(GameManager.instance.currentPlayer != null && GameManager.instance.currentPlayer.ID == m.GetEntityId())
                {
                    return;
                }*/

                //SHould be a stop message
                if (m.GetActionType() == 1) {
                    e.StopMovement();
                }
                else {

                    if (Vector2.Distance(e.transform.position, new Vector2(m.GetXCoordinate(), m.GetYCoordinate())) > 2) {
                        Debug.Log("Entity " + m.GetEntityId() + " is far away from server position. Distance is " + Vector2.Distance(e.transform.position, new Vector2(m.GetXCoordinate(), m.GetYCoordinate())));
                        e.ForceMove(m.GetXCoordinate(), m.GetYCoordinate());
                    }

                    if (GameManager.instance.currentPlayer == null || GameManager.instance.currentPlayer.ID != m.GetEntityId()) {
                        e.DirectMove(m.GetXCoordinate(), m.GetYCoordinate(), m.GetXVelocity(), m.GetYVelocity());
                    }
                }

                e.LastUpdate = m.GetSequenceNumber();
            }));
        }

        public void Visit(AttackMessage m)
        {
            Debug.Log(m);
        }

        public void Visit(EntityUpdateMessage m)
        {
            Debug.Log(m);
            if(m.GetEntityAction() == EntityUpdateMessage.Action.CREATE)
            {
                if(GameManager.instance.Entities.ContainsKey(m.GetEntityID()))
                {
                    return;
                }

                if(m.GetEntityType() == EntityUpdateMessage.Type.PLAYER)
                {
                    GameManager.instance.TaskQueue.Enqueue(new Action(() => {
                        // Refactor out!
                        Player p = (Player) GameObject.Instantiate(Resources.Load<GameObject>("Player")).GetComponent<Player>();
                        p.ID = m.GetEntityID();
                        GameManager.instance.Entities.TryAdd(p.ID, p);
                    }));
                }

                if(m.GetEntityType() == EntityUpdateMessage.Type.ENEMY)
                {
                    GameManager.instance.TaskQueue.Enqueue(new Action(() => {
                        // Refactor out!
                        Enemy e = (Enemy) GameObject.Instantiate(Resources.Load<GameObject>("Enemy")).GetComponent<Enemy>();
                        e.ID = m.GetEntityID();
                        GameManager.instance.Entities.TryAdd(e.ID, e);
                    }));
                }
                if(m.GetEntityType() == EntityUpdateMessage.Type.WEAPON_CROSSBOW) 
                {
                    GameManager.instance.TaskQueue.Enqueue(new Action(() => {
                        // Refactor out!
                        Weapon w = GameObject.Instantiate(Resources.Load<GameObject>("Crossbow")).GetComponent<Crossbow>();
                        w.ID = m.GetEntityID();
                        GameManager.instance.Entities.TryAdd(w.ID, w);
                    }));
                }
                if (m.GetEntityType() == EntityUpdateMessage.Type.WEAPON_BOW) {
                    GameManager.instance.TaskQueue.Enqueue(new Action(() => {
                        // Refactor out!
                        Weapon w = GameObject.Instantiate(Resources.Load<GameObject>("Bow")).GetComponent<Bow>();
                        w.ID = m.GetEntityID();
                        GameManager.instance.Entities.TryAdd(w.ID, w);
                    }));
                }


            }
            else if(m.GetEntityAction() == EntityUpdateMessage.Action.CONTROL)
            {
                GameManager.instance.TaskQueue.Enqueue(new Action(() => {
                    GameManager.instance.Entities.TryGetValue((UInt32)m.GetEntityID(), out Entity e);
                    Player p = (Player)e;
                    p.Controllable = true;
                    GameManager.instance.currentPlayer = p;

                    p.gameObject.AddComponent<InputHandler>();
                    GameObject.FindGameObjectWithTag("MainCamera").AddComponent<Follow>();
                    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Follow>().SetTarget(p.gameObject);
                    GameObject visionCone = GameObject.Instantiate(Resources.Load<GameObject>("VisionCone"));
                    visionCone.GetComponent<Follow>().SetTarget(p.gameObject);
                    GameManager.instance.cone = visionCone.GetComponent<VisionCone>();
                }));
            }
        }

        public void Visit(ItemPickupMessage m) {
            // If the pickup was accepted
            if (m.GetPickupConfirmed() == 1) {
                GameManager.instance.Entities.TryGetValue((UInt32)m.GetPickupItemId(), out Entity e);
                Weapon w = (Weapon)e;
                GameManager.instance.Entities.TryGetValue((UInt32)m.GetEntityId(), out e);
                Player p = (Player)e;

                Debug.Log("Before calling gamemanager");
                GameManager.instance.TaskQueue.Enqueue(new Action(() => {
                    p.GrabWeapon(w);
                    GameManager.instance.DestroyEntityID(m.GetPickupItemId());
                }));
            }
        }

        public void Visit(StateUpdateMessage m) {
            Debug.Log("Player got a StateUpdateMessage");
            if (m.GetUpdateDescriptor() == StateUpdateMessage.Descriptor.DAY) {
                Debug.Log("It was a DAY message");
                GameManager.instance.TaskQueue.Enqueue(new Action(() => {
                    GameManager.instance.cone.hideCone();
                }));
                
            } else if (m.GetUpdateDescriptor() == StateUpdateMessage.Descriptor.NIGHT) {
                Debug.Log("It was a NIGHT message");
                GameManager.instance.TaskQueue.Enqueue(new Action(() => {
                    GameManager.instance.cone.showCone();
                }));
            }
        }

        public void Visit(Message m) { Debug.Log(m); }
    }
}
