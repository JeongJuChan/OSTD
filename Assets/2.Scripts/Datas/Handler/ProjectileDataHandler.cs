using System.Collections;
using System.Collections.Generic;
//using UnityEditor.iOS;
using UnityEngine;

public class ProjectileDataHandler
{
    private Dictionary<int, Projectile> projectileDict;

    private GameData projectileData;

    #region Initialize
    public void Init()
    {
        projectileDict = new Dictionary<int, Projectile>();
        projectileDict.Clear();

        projectileData = Resources.Load<GameData>($"{Consts.GAME_DATA}/ProjectileData");
        List<SerializableRow> rows = projectileData.GetDataRows();

        for (int i = 0; i < rows.Count; i++)
        {
            List<string> elements = rows[i].rowData;
            int index = int.Parse(elements[0]);
            string name = elements[1];
            Projectile projectile = Resources.Load<Projectile>($"Projectile/{name}");
            projectile.SetIndex(index);
            if (!projectileDict.ContainsKey(index))
            {
                projectileDict.Add(index, projectile);
            }
        }
    }   
    #endregion

    #region ProjectileResource
    public Projectile GetProjectileData(int index)
    {
        if (projectileDict.ContainsKey(index))
        {
            return projectileDict[index];
        }

        return default;
    }
    #endregion
}
