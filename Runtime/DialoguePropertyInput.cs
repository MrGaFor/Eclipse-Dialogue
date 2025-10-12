using UnityEngine;
using Sirenix.OdinInspector;

namespace EC.Dialogue
{
    [HideMonoScript]
    public class DialoguePropertyInput : MonoBehaviour
    {
        [SerializeField, BoxGroup()] private DialogueContainer _container;
        [SerializeField, BoxGroup()] private string _propertyName;

        public void SetInt(int value) => _container.SetProperty(_propertyName, value);
        public void SetFloat(float value) => _container.SetProperty(_propertyName, value);
        public void SetString(string value) => _container.SetProperty(_propertyName, value);
        public void SetBool(bool value) => _container.SetProperty(_propertyName, value);
    }
}