using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS.Environment
{
    public class DayNightCycle : MonoBehaviour
    {
        //Inspector exposed
        [SerializeField][Range(0, 24)] private float timeOfDay;
        [SerializeField] private float timeSpeed = 1f;

        //Private
        [SerializeField] private Light sun;
        //[SerializeField] private Light moon;
       

        //Public      
        public float TimeOfDay { get { return timeOfDay; } }

        private void Start()
        {
            sun = GameObject.Find("Sun").GetComponent<Light>();
           // moon = GameObject.Find("Moon").GetComponent<Light>();
        }


        private void OnValidate()
        {
            UpdateLighting(timeOfDay / 24);
        }

        void Update()
        {
            timeOfDay += Time.deltaTime * timeSpeed;
            timeOfDay %= 24;
            UpdateLighting(timeOfDay / 24);
        }



        private void UpdateLighting(float timePercent)
        {
            sun.transform.rotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, -170, 0));

            //moon.transform.rotation = Quaternion.Euler(new Vector3((timePercent * 360f) + 90f, -170, 0));
        }
    }
}


