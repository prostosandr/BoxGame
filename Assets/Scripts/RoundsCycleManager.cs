using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundsCycleManager : MonoBehaviour
{
    [SerializeField] private BoxSpawner _spawner;
    [SerializeField] private Interactor _interactor;
    [SerializeField] private TextMeshProUGUI _quotaText;
    [SerializeField] private TextMeshProUGUI _roundsText;
    [SerializeField] private TextMeshProUGUI _loseText;
    [SerializeField] private TextMeshProUGUI _winText;

    [SerializeField] private HandMover _leftHand;
    [SerializeField] private HandMover _rightHand;

    [SerializeField] private int _quota;
    [SerializeField] private int _rounds;

    [SerializeField] private TableButton _openAllBoxesButton;
    [SerializeField] private TableButton _nextRoundButton;

    private float _currentQuota;
    private int _currentRound;

    private bool _roundComplete;
    private bool _isOneHandDestoyed;

    private void OnEnable()
    {
        _spawner.AllBoxSmashed += ActivateNextRoundButton;
        _interactor.Intearacted += CheckObject;

        _leftHand.Destroyed += CheckCanStopGame;
        _rightHand.Destroyed += CheckCanStopGame;

        _openAllBoxesButton.Clicked += OpenAllBoxes;
        _nextRoundButton.Clicked += NextRound;
    }

    private void Start()
    {
        _currentQuota = 0;
        _currentRound = 1;

        _roundComplete = false;

        RefreshText();

        _spawner.Spawn();

        _openAllBoxesButton.Open();
        _nextRoundButton.Close();
    }

    private void OnDisable()
    {
        _spawner.AllBoxSmashed -= ActivateNextRoundButton;
        _interactor.Intearacted -= CheckObject;

        _leftHand.Destroyed -= CheckCanStopGame;
        _rightHand.Destroyed -= CheckCanStopGame;

        _openAllBoxesButton.Clicked -= OpenAllBoxes;
        _nextRoundButton.Clicked -= NextRound;
    }

    public void NextRound(TableButton button)
    {
        _spawner.Clear();

        _currentRound++;

        if (_currentRound > _rounds)
        {
            if (_currentQuota < _quota)
            {
                _loseText.gameObject.SetActive(true);
            }
            else
            {
                _winText.gameObject.SetActive(true);
            }
        }
        else
        {
            _spawner.Spawn();

            RefreshText();
        }

        _roundComplete = false;

        _openAllBoxesButton.Open();
        button.Close();
    }

    public void OpenAllBoxes(TableButton button)
    {
        if (_roundComplete)
            return;

        foreach(Box box in _spawner.ActiveBox)
        {
            if(box.IsOpened == false)
            {
                box.Open();
            }
        }

        _roundComplete = true;

        button.Close();
        _nextRoundButton.Open();
    }

    private void ActivateNextRoundButton()
    {
        _nextRoundButton.Open();
        _openAllBoxesButton.Close();

        _roundComplete = true;
    }

    private void CheckObject(IInteract interactObject)
    {
        if (interactObject is Box box)
            CountQuota(box);
    }

    private void CountQuota(Box box)
    {
        if (box.CurrentBoxContent.ContentType == BoxContentType.Empty)
            return;

        if (box.CurrentBoxContent.ContentType == BoxContentType.Trap)
        {
            Trap trap = box.GetTrap();
            _currentQuota -= trap.Penalty;
        }

        if (box.CurrentBoxContent.ContentType == BoxContentType.Reward)
        {
            Reward reward = box.GetReward();

            if (reward != null)
                _currentQuota += reward.Price;
        }

        RefreshText();
    }

    private void RefreshText()
    {
        _quotaText.text = ($"Quota: {_currentQuota}/{_quota}");
        _roundsText.text = ($"Round: {_currentRound}/{_rounds}");
    }

    private void CheckCanStopGame()
    {
        if(_isOneHandDestoyed)
        {
            _loseText.gameObject.SetActive(true);
        }
        else
        {
            _isOneHandDestoyed = true;
        }
    }
}
