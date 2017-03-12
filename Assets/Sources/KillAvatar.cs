using UnityEngine;

public class KillAvatar : MonoBehaviour {
    private AvatarController findAvatar(GameObject go) {
        var avatar = go.GetComponent<AvatarController>();
        if (avatar == null && go.transform.parent != null) {
            return findAvatar(go.transform.parent.gameObject);
        }
        return avatar;
    }

    public void Bury() {
        var avatar = findAvatar(gameObject);
        if (avatar != null) {
            avatar.Bury();
        }
    }
}

