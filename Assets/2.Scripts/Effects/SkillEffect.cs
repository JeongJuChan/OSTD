using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] skillEffects;

    public void SetColor(Color32 color)
    {
        ParticleSystem.ColorOverLifetimeModule buildModule = skillEffects[0].colorOverLifetime;
        Gradient buildGradient = new Gradient();
        buildGradient.SetKeys(new GradientColorKey[] {
            new GradientColorKey(color, skillEffects[0].colorOverLifetime.color.gradient.colorKeys[0].time),
            new GradientColorKey(color, skillEffects[0].colorOverLifetime.color.gradient.colorKeys[1].time) },
            skillEffects[0].colorOverLifetime.color.gradient.alphaKeys);
        buildModule.color = buildGradient;

        ParticleSystem.ColorOverLifetimeModule shorkWaveModule = skillEffects[1].colorOverLifetime;
        Gradient shorkWaveGradient = new Gradient();
        shorkWaveGradient.SetKeys(new GradientColorKey[] { 
            new GradientColorKey(color, skillEffects[1].colorOverLifetime.color.gradient.colorKeys[0].time), 
            new GradientColorKey(color, skillEffects[1].colorOverLifetime.color.gradient.colorKeys[1].time) }, 
            skillEffects[1].colorOverLifetime.color.gradient.alphaKeys);
        shorkWaveModule.color = shorkWaveGradient;

        ParticleSystem.ColorOverLifetimeModule cylinderModule = skillEffects[2].colorOverLifetime;
        Gradient cylinderGradient = new Gradient();
        cylinderGradient.SetKeys(new GradientColorKey[] { 
            new GradientColorKey(color, skillEffects[2].colorOverLifetime.color.gradient.colorKeys[0].time), 
            new GradientColorKey(color, skillEffects[2].colorOverLifetime.color.gradient.colorKeys[1].time) },
            skillEffects[2].colorOverLifetime.color.gradient.alphaKeys);
        cylinderModule.color = cylinderGradient;
    }

    public void SetParticlePosY(float posY)
    {
        Vector2 pos = skillEffects[0].transform.localPosition;
        pos.y = posY;
        skillEffects[0].transform.localPosition = pos;
    }

    public void Play()
    {
        skillEffects[0].Play();
    }
}
