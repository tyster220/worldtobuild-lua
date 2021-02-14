using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;


[BluaClass(description = "An object representing a player in the world")]
public class LuaPlayer: LuaObject
{
    [MoonSharpHidden]
    public PhotonPlayer playerConnection;

    [MoonSharpHidden]
    public GameObject playerObject
    {
        get {
            return playerConnection.Character;
        }
    }

    public LuaPlayer(PhotonPlayer _connection)
    {
        this.type = "Player";
        if (_connection != null)
        {
            playerConnection = _connection;
            Task.luaPlayers.Add(playerConnection.ID, this);
        }
    }

    [BluaMethod(description = "Returns whether or not the {object} is a host", scriptSide = ScriptSide.Any)]
    public bool IsHost()
    {
        return playerConnection.IsMasterClient;
    }

    float? originalAddedGravity = null;
    [BluaProperty(description = "Whether or not this {object} is affected by gravity")]
    public bool gravityEnabled
    {
        get
        {
            return _gravityEnabled;
        }
        set
        {
            if (originalAddedGravity == null)
            {
                originalAddedGravity = playerObject.GetComponent<CharacterController>().addedGravity;
            }

            _gravityEnabled = value;

            if (_gravityEnabled)
            {
                playerObject.GetComponent<CharacterController>().addedGravity = originalAddedGravity;
                playerObject.GetComponent<Rigidbody>().useGravity = true;
            }
            else
            {
                
                playerObject.GetComponent<CharacterController>().addedGravity = 0;
                playerObject.GetComponent<Rigidbody>().useGravity = false;
            }
        }
    }
    bool _gravityEnabled = true;

    [BluaProperty(description = "The network ID of the {object}", deprecated = true, deprecatedMessage = "Please use the netID property instead")]
    public int id
    {
        get
        {
            return this.netID;
        }
    }

    [BluaProperty(description = "The network ID of the {object}")]
    public int netID
    {
        get
        {
            if (playerObject != null)
            {
                if (playerObject.GetComponent<PhotonView>() != null)
                {
                    if (playerObject.GetComponent<PhotonView>().owner != null)
                    {
                        return playerObject.GetComponent<PhotonView>().owner.ID;
                    }
                    else
                    {
                        return -1;
                    }

                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
    }

    [BluaProperty(description = "The user ID of the {object}")]
    public int userID
    {
        get
        {
            return playerConnection.UserID;
        }
    }

    [BluaProperty(description = "If true, the {object} will not be hit by raycasts")]
    public bool ignoreRaycast
    {
        get
        {
            return _ignoreRaycast;
        }
        set
        {
            if (value)
            {
                playerObject.layer = 2;
            }
            else
            {
                playerObject.layer = 0;
            }
            _ignoreRaycast = value;
        }
    }
    public bool _ignoreRaycast = false;

    [BluaProperty(description = "The nickname of the {object}")]
    public string name
    {
        get
        {
            return playerObject.GetComponent<PhotonView>().owner.Nickname;
        }
    }

    [BluaProperty(description = "The username of the {object}")]
    public string username
    {
        get
        {
            return playerObject.GetComponent<PhotonView>().owner.Username;
        }
    }

    [BluaProperty(description = "If false, the player's nametag will be hidden")]
    public bool showTag
    {
        get
        {
            return NametagController.instance.GetNametagHidden(playerObject.GetComponent<PhotonView>().owner);
        }
        set
        {
            NametagController.instance.SetNametagHidden(playerObject.GetComponent<PhotonView>().owner, value);
        }
    }

    [BluaProperty(description = "The forward direction vector of the {object}'s look angle")]
    public Vector3 viewForward
    {
        get
        {
            //Quaternion doesnt multiply with the * operator, it does magic instead, this is the forward direction
            return playerObject.GetComponent<ViewDirectionSync>().aimQuat * Vector3.forward;
        }
    }

    [BluaProperty(description = "The backward direction vector of the {object}'s look angle")]
    public Vector3 viewBackward
    {
        get
        {
            return -viewForward;
        }
    }

    [BluaProperty(description = "The right direction vector of the {object}'s look angle")]
    public Vector3 viewRight
    {
        get
        {
            //Quaternion doesnt multiply with the * operator, it does magic instead, this is the forward direction
            return playerObject.GetComponent<ViewDirectionSync>().aimQuat * Vector3.right;
        }
    }

    [BluaProperty(description = "The left direction vector of the {object}'s look angle")]
    public Vector3 viewLeft
    {
        get
        {
            return -viewRight;
        }
    }

    [BluaProperty(description = "The up direction vector of the {object}'s look angle")]
    public Vector3 viewUp
    {
        get
        {
            //Quaternion doesnt multiply with the * operator, it does magic instead, this is the forward direction
            return playerObject.GetComponent<ViewDirectionSync>().aimQuat * Vector3.up;
        }
    }

    [BluaProperty(description = "The down direction vector of the {object}'s look angle")]
    public Vector3 viewDown
    {
        get
        {
            return -viewUp;
        }
    }

    [BluaProperty(description = "The {object}'s look angle")]
    public Vector3 viewAngles
    {
        get
        {
            Quaternion quat = playerObject.GetComponent<ViewDirectionSync>().aimQuat;
            return quat.eulerAngles;
        }
    }

    [BluaProperty(description = "The position of the {object}'s view")]
    public Vector3? viewPosition
    {
        get
        {
            Vector3? pos = position;
            if (pos == null) return null;
            return (Vector3)pos + (Vector3.up * 0.8f * playerObject.transform.localScale.y);
        }
    }

    [BluaProperty(description = "The position of the {object}")]
    public Vector3? position
    {
        get
        {
            if (playerObject== null)
            {
                return null;
            }
            
            return playerObject.transform.position;
        }
        set
        {
            if (playerObject != null)
            {
                Vector3 newPos = (Vector3)value;
                playerObject.transform.position = newPos;
            }
        }
    }
    
    [BluaMethod(description = "Sets the position of the player", scriptSide = ScriptSide.Server)]
    public void HostSetPosition(Vector3 newPosition)
    {
        if (PhotonNetwork.isMasterClient)
        {
            Task.handler.photonView.RPC("RPCReceiveLuaPlayerPosition", playerObject.GetPhotonView().owner, newPosition);
        }
    }

    [BluaProperty(description = "The screen position of this {object}")]
    public Vector2 screenPosition
    {
        get
        {
            return Camera.main.WorldToScreenPoint((playerObject.transform.position));
        }
    }

    [BluaProperty(description = "The rotation of this {object}")]
    public Vector3 angles
    {
        get
        {
            return playerObject.transform.rotation.eulerAngles;
        }
        set
        {
            playerObject.transform.rotation = Quaternion.Euler(value);
        }
    }

    [BluaProperty(description = "Returns the forward direction vector of this {object}")]
    public Vector3 forward
    {
        get
        {
            return playerObject.transform.forward;
        }
    }

    [BluaProperty(description = "Returns the backward direction vector of this {object}")]
    public Vector3 backward
    {
        get
        {
            return -forward;
        }
    }

    [BluaProperty(description = "Returns the right direction vector of this {object}")]
    public Vector3 right
    {
        get
        {
            return playerObject.transform.right;
        }
    }

    [BluaProperty(description = "Returns the left direction vector of this {object}")]
    public Vector3 left
    {
        get
        {
            return -right;
        }
    }

    [BluaProperty(description = "Returns the up direction vector of this {object}")]
    public Vector3 up
    {
        get
        {
            return playerObject.transform.up;
        }
    }

    [BluaProperty(description = "Returns the down direction vector of this {object}")]
    public Vector3 down
    {
        get
        {
            return -up;
        }
    }

    [BluaProperty(description = "The size of the {object}")]
    public Vector3 size
    {
        get
        {
            return playerObject.transform.lossyScale;
        }
        set
        {
            // set global scale
            Transform formerparent = playerObject.transform.parent;
            playerObject.transform.parent = null;
            playerObject.transform.localScale = value;
            playerObject.transform.parent = formerparent;
        }
    }

    [BluaMethod(description = "Sets the size of the player", scriptSide = ScriptSide.Server)]
    public void HostSetSize(Vector3 newSize)
    {
        if (PhotonNetwork.isMasterClient)
        {
            Task.handler.photonView.RPC("RPCReceiveLuaPlayerSize", playerObject.GetPhotonView().owner, newSize);
        }
    }

    [BluaProperty(description = "The velocity of the {object}")]
    public Vector3 velocity
    {
        get
        {
            return playerObject.gameObject.GetComponent<Rigidbody>().velocity;
        }
        set
        {
            //playerObject.movementVector = Vector3.zero;
            playerObject.gameObject.GetComponent<Rigidbody>().velocity = value;
        }
    }

    [BluaMethod(description = "Sets the velocity of the {object}", scriptSide = ScriptSide.Server)]
    public void HostSetVelocity(Vector3 velocity)
    {
        if (PhotonNetwork.isMasterClient)
        {
            Task.handler.photonView.RPC("RPCReceiveLuaPlayerVelocity", playerObject.GetPhotonView().owner, velocity);
        }
    }
    
    [BluaProperty(description = "The mass of the {object}")]
    public float mass
    {
        get
        {
            return playerObject.GetComponent<Rigidbody>().mass;
        }
        set
        {
            playerObject.GetComponent<Rigidbody>().mass = value;
        }
    }
    
    [BluaProperty(description = "If true, the {object} will be unable to move and wont be affected by physics")]
    public bool frozen
    {
        get
        {
            return playerObject.GetComponent<Rigidbody>().isKinematic;
        }
        set
        {
            playerObject.GetComponent<Rigidbody>().isKinematic = value;
        }
    }

    [BluaMethod(description = "If set to true, the {object} will be unable to move and wont be affected by physics", scriptSide = ScriptSide.Server)]
    public void HostSetFrozen(bool frozen)
    {
        if (PhotonNetwork.isMasterClient)
        {
            Task.handler.photonView.RPC("RPCReceiveLuaPlayerFrozen", playerObject.GetPhotonView().owner, frozen);
        }
    }

    [BluaProperty(description = "If set to false, the {object} will be invisible")]
    public bool visible
    {
        get
        {
            return playerObject.GetComponentInChildren<Renderer>().enabled;
        }
        set
        {
            playerObject.GetComponentInChildren<Renderer>().enabled = value;
        }
    }

    [BluaMethod(description = "If set to false, the {object} will be invisible", scriptSide = ScriptSide.Server)]
    public void HostSetVisible(bool visible)
    {
        if (PhotonNetwork.isMasterClient)
        {
            Task.handler.photonView.RPC("RPCReceiveLuaPlayerVisible", playerObject.GetPhotonView().owner, visible);
        }
    }

    [BluaProperty(description = "If set to false, the {object} will not collide with other objects")]
    public bool canCollide
    {
        get
        {
            return !playerObject.GetComponent<Collider>().isTrigger;
        }
        set
        {
            playerObject.GetComponent<Collider>().isTrigger = !value;
        }
    }

    [BluaMethod(description = "If set to false, the {object} will not collide with other objects", scriptSide = ScriptSide.Server)]
    public void HostSetCanCollide(bool cancollide)
    {
        if (PhotonNetwork.isMasterClient)
        {
            Task.handler.photonView.RPC("RPCReceiveLuaPlayerCanCollide", playerObject.GetPhotonView().owner, cancollide);
        }
    }

    [BluaProperty(description = "The movement speed of the {object}")]
    public float speed
    {
        get
        {
            return playerObject.GetComponent<CharacterController>().absoluteSpeed;
        }
        set
        {

            playerObject.GetComponent<CharacterController>().absoluteSpeed = value;
        }
    }

    [BluaMethod(description = "Sets the movement speed of the {object}", scriptSide = ScriptSide.Server)]
    public void HostSetSpeed(float speed)
    {
        if (PhotonNetwork.isMasterClient)
        {
            Task.handler.photonView.RPC("RPCReceiveLuaPlayerSpeed", playerObject.GetPhotonView().owner, speed);
        }
    }

    [BluaProperty(description = "How fast the {object} can stop movement")]
    public float brakingPower
    {
        get
        {
            return playerObject.GetComponent<CharacterController>().brakingPower;
        }
        set
        {
            playerObject.GetComponent<CharacterController>().brakingPower = value;
        }
    }

    [BluaMethod(description = "Sets how fast the {object} can stop movement", scriptSide = ScriptSide.Server)]
    public void HostSetBrakingPower(float brakingpower)
    {
        if (PhotonNetwork.isMasterClient)
        {
            Task.handler.photonView.RPC("RPCReceiveLuaPlayerBrakingPower", playerObject.GetPhotonView().owner, brakingpower);
        }
    }
}