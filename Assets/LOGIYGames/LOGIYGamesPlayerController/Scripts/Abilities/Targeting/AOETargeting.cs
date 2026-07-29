using LOGIYGames.CharacterCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class AOETargeting : AbilityTargetingStrategy
{
    public GameObject aoePrefab;
    public float aoeRadius = 5f;
    public LayerMask groundLayerMask = 1;
    public LayerMask targetLayerMask = ~0;

    private GameObject previewInstance;
    private GameObject radiusGhost;

    public override void Start(Ability ability, AbilityTargetingController targetingManager)
    {
        this.ability = ability;
        this.targetingManager = targetingManager;

        Cancel();

        isTargeting = true;
        targetingManager.SetCurrentStrategy(this);

        // Основной префаб без масштабирования
        if (aoePrefab != null)
        {
            previewInstance = UnityEngine.Object.Instantiate(
                aoePrefab,
                Vector3.up * 0.01f,
                Quaternion.identity);

            CreateRadiusGhost();
        }
    }

    private void CreateRadiusGhost()
    {
        // Стандартный меш Unity - Cylinder
        radiusGhost = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        radiusGhost.name = "AOE Radius Ghost";

        // Делаем дочерним объектом preview
        radiusGhost.transform.SetParent(previewInstance.transform, false);

        // Удаляем коллайдер
        UnityEngine.Object.Destroy(radiusGhost.GetComponent<Collider>());

        // Располагаем чуть ниже префаба
        radiusGhost.transform.localPosition = Vector3.zero;

        // Радиус цилиндра = 0.5, поэтому масштаб = radius * 2
        radiusGhost.transform.localScale = new Vector3(
            aoeRadius * 2f,
            0.01f,
            aoeRadius * 2f);

        var renderer = radiusGhost.GetComponent<MeshRenderer>();

        // Простой полупрозрачный красный материал
        Material mat = new Material(Shader.Find("Standard"));

        mat.color = new Color(1f, 0f, 0f, 0.35f);

        // Настройка прозрачности Standard shader
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;

        renderer.material = mat;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    public override void Update()
    {
        if (!isTargeting || previewInstance == null)
            return;

        Vector3 mousePos = GetMouseWorldPosition();
        previewInstance.transform.position = mousePos + Vector3.up * 0.1f;

        // ЛКМ - применить способность
        if (Input.GetMouseButtonDown(0))
        {
            Collider[] hits = Physics.OverlapSphere(
                mousePos,
                aoeRadius,
                targetLayerMask);

            HashSet<Character> targets = new();

            foreach (Collider hit in hits)
            {
                Character character = hit.GetComponentInParent<Character>();

                if (character != null)
                    targets.Add(character);
            }

            foreach (Character target in targets)
            {
                ability.Execute(target);
            }

            Cancel();
            ability.CooldownTimer.Start();
        }

        // ПКМ - отмена
        if (Input.GetMouseButtonDown(1))
        {
            Cancel();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        if (targetingManager == null)
            return Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayerMask))
            return hit.point;

        return Vector3.zero;
    }

    public override void Cancel()
    {
        isTargeting = false;

        if (targetingManager != null)
            targetingManager.ClearCurrentStrategy();

        if (previewInstance != null)
        {
            UnityEngine.Object.Destroy(previewInstance);
            previewInstance = null;
        }

        radiusGhost = null;
    }
}